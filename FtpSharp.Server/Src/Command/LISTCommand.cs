using System;
using System.IO;
using System.Text;

namespace FtpSharp.Server.Command
{
    public sealed class LISTCommand : ICommand
    {
        private ClientObject _clientObject;

        public LISTCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            Console.WriteLine("client send LIST command");

            var targetPath = "";

            if (args.Length > 0)
            {
                var arg = args[0];
                arg = MessageUtil.TrimCRLF(arg);
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

            if (_clientObject.DataConn.Client() != null)
            {
                if (_clientObject.DataConn.Client().Connected)
                {
                    _clientObject.DataConn.Client().GetStream().Write(formattedBytes, 0, formattedBytes.Length);
                    _clientObject.DataConn.Close();
                    _clientObject.DataConn = null;
                }
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
                var pathParts = f.Split("/");
                var lastPath = pathParts[pathParts.Length - 1];
                sb.Append(lastPath);
                sb.Append("\r\n");
            }

            return sb.ToString();
        }

        private string FormatFileInfo(string[] files)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--------------- File Info ----------------");
            sb.Append("\r\n");

            foreach(var f in files)
            {
                var pathParts = f.Split("/");
                var lastPath = pathParts[pathParts.Length - 1];
                sb.Append(lastPath);
                sb.Append("\r\n");
            }

            sb.Append("------------------------------------------");
            sb.Append("\r\n");

            return sb.ToString();
        }
    }
}