using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FtpSharp.Server 
{
    public class Server : IDisposable
    {
        public static ManualResetEvent acceptDone = new ManualResetEvent(false);

        // public static ManualResetEvent sendDone = new ManualResetEvent(false);

        // public static ManualResetEvent receiveDone = new ManualResetEvent(false);

        private Socket _listener = null;
        private readonly string _address;
        private readonly int _port;

        private string _rootDir;

        public static bool _isRunning = true;

        public Server(string address, int port, string rootDir)
        {
            _address = address;
            _port = port;

            _rootDir = rootDir;
            Reply.InitReply();
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
                    acceptDone.Reset();

                    Console.WriteLine("waiting client connection...");

                    _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);

                    acceptDone.WaitOne();

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
                acceptDone.Set();

                ClientObject clientObject = new ClientObject(socketClient);

                clientObject.RootDir = _rootDir;

                clientObject.SendInitialMessage();
                
                clientObject.ProcessMessage();
                
                // receiveDone.WaitOne();
            }
        }
    }
}