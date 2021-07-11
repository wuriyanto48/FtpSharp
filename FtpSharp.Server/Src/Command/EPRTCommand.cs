using System;

namespace FtpSharp.Server.Command
{
    // https://datatracker.ietf.org/doc/html/rfc2428

    // EPRT |2|::1|60534|
    public sealed class EPRTCommand : ICommand
    {
        private ClientObject _clientObject;

        public EPRTCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            Console.WriteLine("client send EPRT command");
            var arg = args[0];
            arg = MessageUtil.TrimCRLF(arg);
            
            var separator = arg[0];
            var dataParts = arg.Split(separator);

            bool validAddressFamily = int.TryParse(dataParts[1], out var addressFamily);
            if (!validAddressFamily)
            {
                byte[] invalidAddressFamilyData = MessageUtil.BuildReply(_clientObject, 522, "Network protocol not supported");
                _clientObject.Write(invalidAddressFamilyData);
                return;
            }

            // https://datatracker.ietf.org/doc/html/rfc2428
            // AF Number   Protocol
            // ---------   --------
            // 1           Internet Protocol, Version 4 [Pos81a]
            // 2           Internet Protocol, Version 6 [DH96]
            if (addressFamily != 1 && addressFamily != 2)
            {
                byte[] invalidAddressFamilyData = MessageUtil.BuildReply(_clientObject, 522, "Network protocol not supported");
                _clientObject.Write(invalidAddressFamilyData);
                return;
            }

            var host = dataParts[2];
            var port = int.Parse(dataParts[3]);

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