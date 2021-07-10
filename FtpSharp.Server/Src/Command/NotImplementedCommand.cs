using System;
using System.Net.Sockets;

namespace FtpSharp.Server.Command
{
    public sealed class NotImplementedCommand : ICommand
    {
        private ClientObject _clientObject;

        public NotImplementedCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            byte[] data = MessageUtil.BuildReply(_clientObject, 502);
            _clientObject.Write(data);
        }

        public bool ShouldLogin()
        {
            return false;
        }
    }
}