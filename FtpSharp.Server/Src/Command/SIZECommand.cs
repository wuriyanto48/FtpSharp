using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    public sealed class SIZECommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public SIZECommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<SIZECommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send SIZE command");

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

            // open file
            var size = fileInfo.Length;

            byte[] validListRequestData = MessageUtil.BuildReply(_clientObject, 213, size.ToString());
            _clientObject.Write(validListRequestData);
            
        }

        public bool ShouldLogin()
        {
            return true;
        }

    }
}