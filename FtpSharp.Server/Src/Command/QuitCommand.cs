using System;
using System.Net.Sockets;

namespace FtpSharp.Server.Command
{
    public class QuitCommand : ICommand
    {
        private ClientObject _clientObject;

        public QuitCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            byte[] data = MessageUtil.BuildReply(_clientObject, 221);
            _clientObject.SendMessage(data);
            _clientObject._clientSocket.Shutdown(SocketShutdown.Both);  
            _clientObject._clientSocket.Close();
        }
    }
}