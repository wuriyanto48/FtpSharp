using System;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    public sealed class PASSCommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public PASSCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<PASSCommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send PASS command");
            _logger.LogInformation($"{String.Join(",", args)}");
            var password = args[0];
            password = MessageUtil.TrimCRLF(password);

            var validAuth = _clientObject.Auth.Check(_clientObject.ReqUsername, password);
            if (!validAuth)
            {
                byte[] invalidData = MessageUtil.BuildReply(_clientObject, 530, "Login Error, Invalid username or password");
                _clientObject.Write(invalidData);
                return;
            }

            _clientObject.Username = _clientObject.ReqUsername;
            _clientObject.ReqUsername = "";
            byte[] data = MessageUtil.BuildReply(_clientObject, 230, "Login OK");
            _clientObject.Write(data);
        }

        public bool ShouldLogin()
        {
            return false;
        }
    }
}