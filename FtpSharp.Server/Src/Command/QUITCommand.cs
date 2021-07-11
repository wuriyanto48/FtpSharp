using System;
using System.Net.Sockets;

namespace FtpSharp.Server.Command
{
    public sealed class QUITCommand : ICommand
    {
        private ClientObject _clientObject;

        public QUITCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            // notify quit event handler
            _clientObject.Server._quitEventNotifier.Notify(new QuitEventArgs(_clientObject.SessionID));
            
            byte[] data = MessageUtil.BuildReply(_clientObject, 221);
            _clientObject.Write(data);
            _clientObject._clientSocket.Shutdown(SocketShutdown.Both);  
            _clientObject._clientSocket.Close();
        }

        public bool ShouldLogin()
        {
            return false;
        }
    }
}