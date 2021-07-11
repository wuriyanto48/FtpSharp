using System;

namespace FtpSharp.Server.Command
{
    public sealed class USERCommand : ICommand
    {
        private ClientObject _clientObject;

        public USERCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            Console.WriteLine("client send USER command");
            Console.WriteLine($"{String.Join(",", args)}");
            var username = args[0];
            username = MessageUtil.TrimCRLF(username);

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