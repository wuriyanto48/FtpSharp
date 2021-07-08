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
            byte[] data = MessageUtil.BuildReply(_clientObject, 230);
            _clientObject.SendMessage(data);
        }
    }
}