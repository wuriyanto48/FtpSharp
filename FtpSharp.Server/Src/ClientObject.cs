using System;
using System.Net.Sockets;
using System.Text;
using System.Linq;

namespace FtpSharp.Server
{
    public sealed class ClientObject
    {
        public const int BufferSize = 1024;

        public byte[] Buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();

        public Socket _clientSocket = null;

        public string RootDir  { get; set; }

        public string WorkDir { get; set; }

        public string Username { get; set; }

        public string ReqUsername { get; set; }

        public Auth.IAuth Auth { get; set; }

        public DataType dataType;

        public Socket dataConn;

        private Command.Commands commands;

        public ClientObject(Socket clientSocket)
        {
            _clientSocket = clientSocket;
            dataType = DataType.DEFAULT;

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
                new AsyncCallback(SendCallback), this);
        }

        public void Write(byte[] data)
        {
            _clientSocket.BeginSend(data, 0, data.Length, 0, 
                new AsyncCallback(SendCallback), this);
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
            Console.WriteLine($"bytesRead {bytesRead}");

            // receiveDone.Set();
            String content = String.Empty;
    
            if (bytesRead > 0) {  
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.Buffer, 0, bytesRead));
    
                // Check for end-of-file tag. If it is not there, read
                // more data.  
                content = state.sb.ToString();
                Console.WriteLine($"content {content.Equals("QUIT\r\n")}");
                Console.WriteLine($"content {BitConverter.ToString(Encoding.ASCII.GetBytes(content))}");
                if (content.IndexOf("\n") > -1 || content.IndexOf("\r\n") > -1) {
                    // All the data has been read from the
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",  
                        content.Length, content );  
                    // Echo the data back to the client.  
                    Send(state);
                    
                } else {
                    // Not all data received. Get more.  
                    clientSocket.BeginReceive(state.Buffer, 0, ClientObject.BufferSize, 0,  
                        new AsyncCallback(ReadCallback), state);
                }
            } 
        }

        private void Send(ClientObject state)
        {
            string data = state.sb.ToString();
            string[] messageParts = data.Split(" ");
            Console.WriteLine(String.Join(", ", messageParts));

            string command = messageParts[0];
            string[] args = messageParts.Slice(1, messageParts.Length);
            var isValidCommand = Enum.TryParse(typeof(Command.ECommand), command, false, out var eCommand);
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

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                ClientObject state = (ClientObject) ar.AsyncState;
                Socket clientSocket = state._clientSocket;
                String data = state.sb.ToString();

                // Complete sending the data
                int bytesSent = clientSocket.EndSend(ar); 
                // sendDone.Set(); 
                Console.WriteLine("Sent {0} bytes to client.", bytesSent); 

                ClientObject newState = new ClientObject(clientSocket);

                clientSocket.BeginReceive(newState.Buffer, 0, ClientObject.BufferSize, 0,  
                        new AsyncCallback(ReadCallback), newState);

                // TODO
                // if (data.Equals("QUIT\r\n"))
                // {   
                //     Console.WriteLine("closing connection....");
                //     clientSocket.Shutdown(SocketShutdown.Both);  
                //     clientSocket.Close();
                // } else
                // {
                //     ClientObject newState = new ClientObject(clientSocket);

                //     clientSocket.BeginReceive(newState.Buffer, 0, ClientObject.BufferSize, 0,  
                //             new AsyncCallback(ReadCallback), newState);
                // }

            } catch (Exception e)
            {
                Console.WriteLine($" error {e.StackTrace}");
            }
        }

    }
}