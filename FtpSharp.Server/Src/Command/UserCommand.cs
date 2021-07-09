using System;

namespace FtpSharp.Server.Command
{
    public class UserCommand : ICommand
    {
        private ClientObject _clientObject;

        public UserCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            Console.WriteLine("client send USER command");
            Console.WriteLine($"{String.Join(",", args)}");
            var username = args[0];

            _clientObject.ReqUsername = username;
            byte[] data = MessageUtil.BuildReply(_clientObject, 331);
            _clientObject.SendMessage(data);
        }
    }
}