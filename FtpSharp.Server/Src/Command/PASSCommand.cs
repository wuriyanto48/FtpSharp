using System;

namespace FtpSharp.Server.Command
{
    public class PASSCommand : ICommand
    {
        private ClientObject _clientObject;

        public PASSCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            Console.WriteLine("client send PASS command");
            Console.WriteLine($"{String.Join(",", args)}");
            var password = args[0];

            var validAuth = _clientObject.Auth.Check(_clientObject.ReqUsername, password);
            if (!validAuth)
            {
                byte[] invalidData = MessageUtil.BuildReply(_clientObject, 530, "Login Error, Invalid username or password");
                _clientObject.Write(invalidData);
                return;
            }

            _clientObject.Username = _clientObject.ReqUsername;
            _clientObject.ReqUsername = "";
            byte[] data = MessageUtil.BuildReply(_clientObject, 230, "Login OK");
            _clientObject.Write(data);
        }
    }
}