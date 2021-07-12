using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    // https://datatracker.ietf.org/doc/html/rfc3659#page-8
    public sealed class MDTMCommand : ICommand
    {
        private const string LAST_MODIFIED_FORMAT = "yyyyMMddHHmmss";
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public MDTMCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<MDTMCommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send MDTM command");
            _logger.LogInformation(String.Join(",", args));

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
            var lastModified = fileInfo.LastWriteTime;

            byte[] validListRequestData = MessageUtil.BuildReply(_clientObject, 213, lastModified.ToString(LAST_MODIFIED_FORMAT));
            _clientObject.Write(validListRequestData);
            
        }

        public bool ShouldLogin()
        {
            return true;
        }

    }
}