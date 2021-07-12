using System;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{ 
    // https://datatracker.ietf.org/doc/html/rfc1639
    // LPRT 6,16,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,2,236,118
    public sealed class LPRTCommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public LPRTCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<LPRTCommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send LPRT command");
            _logger.LogInformation($"{String.Join(",", args)}");
            byte[] data = MessageUtil.BuildReply(_clientObject, 200, "Connection estabilished");
            _clientObject.Write(data);
        }

        public bool ShouldLogin()
        {
            return true;
        }
    }
}