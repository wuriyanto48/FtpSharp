using System;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    public sealed class USERCommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public USERCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<USERCommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send USER command");
            _logger.LogInformation($"{String.Join(",", args)}");
            var username = args[0];
            MessageUtil.TrimCRLF(ref username);

            _clientObject.ReqUsername = username;
            byte[] data = MessageUtil.BuildReply(_clientObject, 331);
            _clientObject.Write(data);
        }

        public bool ShouldLogin()
        {
            return false;
        }
    }
}