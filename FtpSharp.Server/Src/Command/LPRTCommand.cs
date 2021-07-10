using System;

namespace FtpSharp.Server.Command
{
    // LPRT 6,16,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,2,236,118
    public sealed class LPRTCommand : ICommand
    {
        private ClientObject _clientObject;

        public LPRTCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            Console.WriteLine("client send LPRT command");
            Console.WriteLine($"{String.Join(",", args)}");
            byte[] data = MessageUtil.BuildReply(_clientObject, 200, "Connection estabilished");
            _clientObject.Write(data);
        }

        public bool ShouldLogin()
        {
            return true;
        }
    }
}