using System;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    public sealed class TYPECommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public TYPECommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<TYPECommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send TYPE command");
            _logger.LogInformation($"{String.Join(",", args)}");
            var dataType = args[0];
            dataType = MessageUtil.TrimCRLF(dataType);
            if (dataType == "A")
            {
                _clientObject.DataType = DataType.ASCII;
                byte[] daresponseSetAsciidatata = MessageUtil.BuildReply(_clientObject, 200, "TYPE set to ASCII");
                _clientObject.Write(daresponseSetAsciidatata);
                return;
            }  
            
            if (dataType == "I")
            {
                _clientObject.DataType = DataType.BINARY;
                byte[] responseSetBinarydata = MessageUtil.BuildReply(_clientObject, 200, "TYPE set to BINARY");
                _clientObject.Write(responseSetBinarydata);
                return;
            }

            byte[] data = MessageUtil.BuildReply(_clientObject, 500, "Input TYPE invalid");
            _clientObject.Write(data);
        }

        public bool ShouldLogin()
        {
            return true;
        }
    }
}