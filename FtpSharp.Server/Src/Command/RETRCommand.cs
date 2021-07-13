using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    public sealed class RETRCommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public RETRCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<RETRCommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send RETR command");

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

            FileInfo fileInfo = new FileInfo(targetPath);
            if (!fileInfo.Exists)
            {
                byte[] invalidCwdRequest = MessageUtil.BuildReply(_clientObject, 550, "File is not exist");
                _clientObject.Write(invalidCwdRequest);
                return;
            }

            byte[] openingConnData = MessageUtil.BuildReply(_clientObject, 150);
            _clientObject.Write(openingConnData);

            // open file
            using FileStream fileStream = fileInfo.OpenRead();

            if (_clientObject.DataConn.IsConnected())
            {
                fileStream.CopyTo(_clientObject.DataConn.Stream());
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

    }
}