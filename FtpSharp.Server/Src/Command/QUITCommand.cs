using System;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    public sealed class QUITCommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public QUITCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<QUITCommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send QUIT command");

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