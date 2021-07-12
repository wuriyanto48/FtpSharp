using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server 
{
    public sealed class Server : IDisposable
    {
        public static ManualResetEvent acceptDone = new ManualResetEvent(false);

        // public static ManualResetEvent sendDone = new ManualResetEvent(false);

        // public static ManualResetEvent receiveDone = new ManualResetEvent(false);

        private Socket _listener = null;

        public Dictionary<string, ClientObject> _clients;

        private Config _config;

        private string _rootDir;

        private Auth.IAuth _auth;

        public static bool _isRunning = true;

        public QuitEventNotifier _quitEventNotifier;
        public QuitEventHandler _quitEventHandler;

        private readonly ILogger _logger;

        public Server(Config config)
        {
            _config = config;
            _rootDir = Path.GetFullPath(_config.RootDir);

            // client database
            _clients = new Dictionary<string, ClientObject>();

            _auth = new Auth.DefaultAuth(config.Secret, config.ServerUsername, config.ServerPassword);

            // register client quit event
            _quitEventNotifier = new QuitEventNotifier();
            _quitEventHandler = new QuitEventHandler(this);
            _quitEventHandler.RegisterNotifier(_quitEventNotifier);

            // init log
            _logger = ApplicationLogging.CreateLogger<Server>();


            Reply.InitReply();
        }

        public void ShowClients()
        {
            Console.WriteLine("------ connected clients: ------");
            foreach(KeyValuePair<string, ClientObject> entry in _clients)
            {
                Console.WriteLine($"- {entry.Key}");
            }

            Console.WriteLine("--------------------------------");
        }

        public void Dispose()
        {
            if (_listener.Connected)
            {
                _logger.LogInformation("closing server {}");
                _listener.Close();
            }
        }

        public void Bind()
        {
            IPHostEntry iPHostEntry = Dns.GetHostEntry(_config.Address);
            IPAddress ipAddress = iPHostEntry.AddressList[0];
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, _config.Port);

            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(iPEndPoint);
                _listener.Listen(10);
            } catch(Exception e)
            {
                _logger.LogError(exception: e, $" error {e.StackTrace}");
            }
        }

        public void Start()
        {
            _logger.LogInformation($"running on port {_config.Port}");
            try
            {

                while (_isRunning)
                {
                    // Set the event to nonsignaled state.  
                    acceptDone.Reset();

                    _logger.LogInformation("waiting client connection...");

                    _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);

                    acceptDone.WaitOne();

                }

                // close server when _isRunning set to false
                _listener.Close();
            } catch (Exception e)
            {
                _logger.LogError(exception: e, $" error {e.StackTrace}");
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

                clientObject.Server = this;

                // add each new client object to database;
                _clients.Add(clientObject.SessionID, clientObject);

                clientObject.RootDir = _rootDir;

                clientObject.Auth = _auth;

                clientObject.WriteInitialMessage();
                
                clientObject.ProcessMessage();

                ShowClients();
                
                // receiveDone.WaitOne();
            }
        }
    }
}