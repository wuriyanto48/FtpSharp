using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    public sealed class LISTCommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public LISTCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<LISTCommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send LIST command");

            var targetPath = "";

            if (args.Length > 0)
            {
                var arg = args[0]; 
                MessageUtil.TrimCRLF(ref arg);
                targetPath = Path.Join(_clientObject.RootDir, _clientObject.WorkDir, arg);
            } else
            {
                targetPath = Path.Join(_clientObject.RootDir, _clientObject.WorkDir);
            }

            DirectoryInfo dir = new DirectoryInfo(targetPath);
            if (!dir.Exists)
            {
                byte[] invalidCwdRequest = MessageUtil.BuildReply(_clientObject, 550, "Directory is not valid");
                _clientObject.Write(invalidCwdRequest);
                return;
            }

            byte[] openingConnData = MessageUtil.BuildReply(_clientObject, 150);
            _clientObject.Write(openingConnData);

            string[] files = Directory.GetFileSystemEntries(targetPath);
            var formatted = FormatFileInfoSimple(files);
            var formattedBytes = Encoding.ASCII.GetBytes(formatted);

            if (_clientObject.DataConn.IsConnected())
            {
                _clientObject.DataConn.Stream().Write(formattedBytes, 0, formattedBytes.Length);
                _clientObject.DataConn.Close();
                _clientObject.DataConn = null;
            }

            byte[] validListRequestData = MessageUtil.BuildReply(_clientObject, 226);
            _clientObject.Write(validListRequestData);
            
        }

        public bool ShouldLogin()
        {
            return true;
        }

        private string FormatFileInfoSimple(string[] files)
        {
            StringBuilder sb = new StringBuilder();

            foreach(var f in files)
            {
                FileInfo fileInfo = new FileInfo(f);
                sb.Append(fileInfo.Name);
                sb.Append("\r\n");
            }

            return sb.ToString();
        }

        private string FormatFileInfo(string[] files)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var f in files)
            {
                FileInfo fileInfo = new FileInfo(f);
                sb.Append($"{fileInfo.Length} {fileInfo.LastWriteTime.ToString("MM _ddHH:mm")} {fileInfo.Name}\r\n");
            }
            return sb.ToString();
        }
    }
}