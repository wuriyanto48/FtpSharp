using System;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    // PORT 127,0,0,1,236,1
    public sealed class PORTCommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public PORTCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<PORTCommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send PORT command");
            
            var arg = args[0];
            arg = MessageUtil.TrimCRLF(arg);

            var parts = arg.Split(",");
            var portSegOne = parts[4];
            var portSegTwo = parts[5];

            var host = String.Format($"{parts[0]}.{parts[1]}.{parts[2]}.{parts[3]}");
            var portSegOneInt = int.Parse(portSegOne);
            var portSegTwoInt = int.Parse(portSegTwo);
            
            var port = (portSegOneInt<<8) + portSegTwoInt;

            _logger.LogInformation($"host: {host}");
            _logger.LogInformation($"port: {port}");

            IDataConnection dataConn = null;
            try
            {
                dataConn = new ActiveDataConnection(host, port);
            } catch (Exception e)
            {
                _logger.LogError(exception: e, $"{e.Message}");
                byte[] invalidAddressFamilyData = MessageUtil.BuildReply(_clientObject, 425);
                _clientObject.Write(invalidAddressFamilyData);
                return;
            }

            _clientObject.DataConn = dataConn;

            byte[] data = MessageUtil.BuildReply(_clientObject, 200, "Connection estabilished");
            _clientObject.Write(data);
        }

        public bool ShouldLogin()
        {
            return true;
        }
    }
}