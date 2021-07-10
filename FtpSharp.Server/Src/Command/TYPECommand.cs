using System;

namespace FtpSharp.Server.Command
{
    public class TYPECommand : ICommand
    {
        private ClientObject _clientObject;

        public TYPECommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            Console.WriteLine("client send TYPE command");
            Console.WriteLine($"{String.Join(",", args)}");
            var dataType = args[0];
            dataType = MessageUtil.TrimCRLF(dataType);
            if (dataType == "A")
            {
                _clientObject.DataType = DataType.ASCII;
                byte[] data = MessageUtil.BuildReply(_clientObject, 200, "TYPE set to ASCII");
                _clientObject.Write(data);
            } else if (dataType == "I")
            {
                _clientObject.DataType = DataType.BINARY;
                byte[] data = MessageUtil.BuildReply(_clientObject, 200, "TYPE set to BINARY");
                _clientObject.Write(data);
            } else
            {
                byte[] data = MessageUtil.BuildReply(_clientObject, 500, "Input TYPE invalid");
                _clientObject.Write(data);
            }
        }

        public bool ShouldLogin()
        {
            return true;
        }
    }
}