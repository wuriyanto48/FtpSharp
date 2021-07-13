using System;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server
{
    public sealed class ClientObject
    {
        public const int BufferSize = 1024;

        public byte[] Buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();

        public string SessionID { get; }

        public Socket _clientSocket = null;

        public string RootDir  { get; set; }

        public string WorkDir { get; set; }

        public string Username { get; set; }

        public string ReqUsername { get; set; }

        public Auth.IAuth Auth { get; set; }

        public DataType DataType { get; set; }

        public IDataConnection DataConn { get; set; }

        private Command.Commands commands;

        public Server Server { get; set; }

        private readonly ILogger _logger;

        public ClientObject(Socket clientSocket)
        {
            _clientSocket = clientSocket;
            DataType = DataType.DEFAULT;

            SessionID = SessionIdGenerator.Generate();

            _logger = ApplicationLogging.CreateLogger<ClientObject>();

            commands = new Command.Commands(this);
            WorkDir = "/";
        }

        public bool IsLogin()
        {
            return !String.IsNullOrEmpty(Username);
        }

        // Checks if the socket is connected
        static bool IsSocketConnected(Socket s)
        {
            try
            {
                return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
            } catch
            {
                return false;
            }
        }

        public void WriteInitialMessage()
        {
            byte[] byteData = MessageUtil.BuildReply(this, 220);
            _clientSocket.BeginSend(byteData, 0, byteData.Length, 0, 
                new AsyncCallback(WriteCallback), this);
        }

        public void Write(byte[] data)
        {
            _clientSocket.BeginSend(data, 0, data.Length, 0, 
                new AsyncCallback(WriteCallback), this);
        }

        public void ProcessMessage()
        {

            _clientSocket.BeginReceive(Buffer, 0, 
                    ClientObject.BufferSize, 0, new AsyncCallback(ReadCallback), this);
        }

        public void ReadCallback(IAsyncResult ar)
        {

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            ClientObject state = (ClientObject) ar.AsyncState;  
            Socket clientSocket = state._clientSocket;

            // check and return from read callback
            if (!IsSocketConnected(clientSocket))
            {
                return;
            } 
    
            // Read data from the client socket.
            int bytesRead = clientSocket.EndReceive(ar);  
            _logger.LogInformation($"bytesRead {bytesRead}");

            // receiveDone.Set();
            String content = String.Empty;
    
            if (bytesRead > 0) {  
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.Buffer, 0, bytesRead));
    
                // Check for end-of-input tag with \n or \r\n. 
                // If it is not there, read more data.  
                content = state.sb.ToString();
                _logger.LogInformation($"content {content}");
                if (content.IndexOf("\n") > -1 || content.IndexOf("\r\n") > -1) {
                    // process and send command data to the client  
                    ProcessCommand(state);
                    
                } else {
                    // Not all data received. Get more.  
                    clientSocket.BeginReceive(state.Buffer, 0, ClientObject.BufferSize, 0,  
                        new AsyncCallback(ReadCallback), state);
                }
            } 
        }

        private void ProcessCommand(ClientObject state)
        {
            string data = state.sb.ToString();
            string[] messageParts = data.Split(" ");
            _logger.LogInformation(String.Join(", ", messageParts));

            string command = messageParts[0];
            command = MessageUtil.TrimCRLF(command);

            string[] args = messageParts.Slice(1, messageParts.Length);
            var isValidCommand = Enum.TryParse(typeof(Command.ECommand), command.ToUpper(), false, out var eCommand);
            if (!isValidCommand)
            {
                var ftpCommand = commands.GetCommand(Command.ECommand.NOTVALID);
                ftpCommand.Process(null);
            } else 
            {
                var ftpCommand = commands.GetCommand((Command.ECommand) eCommand);
                if (ftpCommand.ShouldLogin() && !IsLogin())
                {
                    byte[] notLoginData = MessageUtil.BuildReply(this, 530);
                    Write(notLoginData);
                } else 
                {
                    ftpCommand.Process(args);
                }
            }
            // sendDone.WaitOne();
        }

        private void WriteCallback(IAsyncResult ar)
        {
            try
            {
                ClientObject state = (ClientObject) ar.AsyncState;
                Socket clientSocket = state._clientSocket;
                String data = state.sb.ToString();

                // Complete sending the data
                int bytesSent = clientSocket.EndSend(ar); 
                // sendDone.Set(); 
                _logger.LogInformation("Sent {0} bytes to client.", bytesSent); 

                // client doesn't close its connection
                // then listen more data
                ClientObject newState = new ClientObject(clientSocket);

                clientSocket.BeginReceive(newState.Buffer, 0, ClientObject.BufferSize, 0,  
                        new AsyncCallback(ReadCallback), newState);

            } catch (Exception e)
            {
                _logger.LogError(exception: e, $"error {e.StackTrace}");
            }
        }

    }
}