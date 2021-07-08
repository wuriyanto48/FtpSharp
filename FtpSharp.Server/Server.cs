using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FtpSharp.Server 
{
    public static class MessageUtil
    {
        public static readonly byte[] CarriageReturn = new byte[] {13, 10};
        public static readonly byte[] NewLine = new byte[] {10};
    }

    public class Server : IDisposable
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static ManualResetEvent sendDone = new ManualResetEvent(false);

        public static ManualResetEvent receiveDone = new ManualResetEvent(false);

        private Socket _listener = null;
        private readonly string _address;
        private readonly int _port;

        public static bool _isRunning = true;

        public Server(string address, int port)
        {
            _address = address;
            _port = port;
        }

        public void Dispose()
        {
            if (_listener.Connected)
            {
                Console.WriteLine("closing server {}");
                _listener.Close();
            }
        }

        public void Bind()
        {
            IPHostEntry iPHostEntry = Dns.GetHostEntry(_address);
            IPAddress ipAddress = iPHostEntry.AddressList[0];
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, _port);

            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(iPEndPoint);
                _listener.Listen(10);
            } catch(Exception e)
            {
                Console.WriteLine($" error {e.StackTrace}");
            }
        }

        public void Start()
        {
            try
            {

                while (_isRunning)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    Console.WriteLine("waiting client connection...");

                    _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);

                    allDone.WaitOne();

                }

                // close server when _isRunning set to false
                _listener.Close();
            } catch (Exception e)
            {
                Console.WriteLine($" error {e.StackTrace}");
            }

        }

        private void AcceptCallback(IAsyncResult ar)
        {

            // get client socket
            Socket listener = (Socket) ar.AsyncState;
            if (listener != null)
            {
                Socket socketClient = listener.EndAccept(ar);

                // send signal to thread
                allDone.Set();

                ClientObject clientObject = new ClientObject();
                clientObject.ClientSocket = socketClient;
                socketClient.BeginReceive(clientObject.Buffer, 0, 
                    ClientObject.BufferSize, 0, new AsyncCallback(ReadCallback), clientObject);
                
                // receiveDone.WaitOne();
            }
        }

        public void ReadCallback(IAsyncResult ar)
        {

            String content = String.Empty;
    
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            ClientObject state = (ClientObject) ar.AsyncState;  
            Socket clientSocket = state.ClientSocket;  
    
            // Read data from the client socket.
            int bytesRead = clientSocket.EndReceive(ar);  
            Console.WriteLine($"bytesRead {bytesRead}");

            // receiveDone.Set();
    
            if (bytesRead > 0) {  
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(  
                    state.Buffer, 0, bytesRead));  
    
                // Check for end-of-file tag. If it is not there, read
                // more data.  
                content = state.sb.ToString();  
                if (content.IndexOf("\n") > -1) {
                    // All the data has been read from the
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",  
                        content.Length, content );  
                    // Echo the data back to the client.  
                    Send(clientSocket, state); 
                    
                } else {
                    // Not all data received. Get more.  
                    clientSocket.BeginReceive(state.Buffer, 0, ClientObject.BufferSize, 0,  
                        new AsyncCallback(ReadCallback), state);
                }
            } else 
            {
                clientSocket.Close();
            }  
        }

        private void Send(Socket clientSocket, ClientObject state)
        {
            String data = state.sb.ToString();
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            clientSocket.BeginSend(byteData, 0, byteData.Length, 0, 
                new AsyncCallback(SendCallback), state);
            // sendDone.WaitOne();
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                ClientObject state = (ClientObject) ar.AsyncState;
                Socket clientSocket = state.ClientSocket;
                String data = state.sb.ToString();

                // Complete sending the data
                int bytesSent = clientSocket.EndSend(ar); 
                // sendDone.Set(); 
                Console.WriteLine("Sent {0} bytes to client.", bytesSent); 

                if (data.Equals("quit\n"))
                {
                    clientSocket.Send(Encoding.ASCII.GetBytes("bye"));
                    clientSocket.Send(new byte[]{10});
                    
                    clientSocket.Shutdown(SocketShutdown.Both);  
                    clientSocket.Close();
                } else
                {
                    ClientObject newState = new ClientObject();
                    newState.ClientSocket = clientSocket;

                    clientSocket.BeginReceive(newState.Buffer, 0, ClientObject.BufferSize, 0,  
                            new AsyncCallback(ReadCallback), newState);
                }

            } catch (Exception e)
            {
                Console.WriteLine($" error {e.StackTrace}");
            }
        }
    }
}