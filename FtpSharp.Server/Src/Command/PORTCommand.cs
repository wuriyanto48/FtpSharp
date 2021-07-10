using System;

namespace FtpSharp.Server.Command
{
    // PORT 127,0,0,1,236,1
    public sealed class PORTCommand : ICommand
    {
        private ClientObject _clientObject;

        public PORTCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            Console.WriteLine("client send PORT command");
            
            var arg = args[0];
            arg = MessageUtil.TrimCRLF(arg);

            var parts = arg.Split(",");
            var portSegOne = parts[4];
            var portSegTwo = parts[5];

            var host = String.Format($"{parts[0]}.{parts[1]}.{parts[2]}.{parts[3]}");
            var portSegOneInt = int.Parse(portSegOne);
            var portSegTwoInt = int.Parse(portSegTwo);
            
            var port = portSegOneInt<<8 + portSegTwoInt;

            Console.WriteLine($"host: {host}");
            Console.WriteLine($"port: {port}");

            IDataConnection dataConn = null;
            try
            {
                dataConn = new DefaultDataConnection(host, port);
            } catch (Exception e)
            {
                Console.WriteLine($"**************** {e.Message}");
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