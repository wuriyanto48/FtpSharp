using System;

namespace FtpSharp.Server.Command
{
    public class LISTCommand : ICommand
    {
        private ClientObject _clientObject;

        public LISTCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            Console.WriteLine("client send LIST command");
            
            var arg = args[0];
            arg = MessageUtil.TrimCRLF(arg);
            
        }

        public bool ShouldLogin()
        {
            return true;
        }
    }
}