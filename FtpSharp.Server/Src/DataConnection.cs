using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace FtpSharp.Server 
{

    public interface IDataConnection
    {
        string Host();

        int Port();

        Stream Stream();

        bool IsConnected();

        void Close();
    }

    public class ActiveDataConnection : IDataConnection
    {
        private string _host { get; set; }

        private int _port { get; set; }

        private Socket _client;

        private NetworkStream _stream;

        public ActiveDataConnection(string host, int port)
        {
            _host = host;
            _port = port;

            IPHostEntry ipHostInfo = Dns.GetHostEntry(_host);  
            IPAddress ipAddress = ipHostInfo.AddressList[0]; 
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, _port);
            _client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _client.Connect(remoteEP);
                _stream = new NetworkStream(_client);
            } catch(SocketException e)
            {
                throw new Exception(e.Message);
            }
        }

        public Stream Stream()
        {
            return _stream;
        }

        public bool IsConnected()
        {
            if (_client != null)
            {
                if (_client.Connected)
                {
                    return true;
                }
            }

            return false;
        }

        public void Close()
        {
            if (_client != null)
            {
                if (_client.Connected)
                {
                    _client.Shutdown(SocketShutdown.Both);
                    _client.Close();
                }
            }

            if (_stream != null)
            {
                _stream.Close();
            }
        }

        public string Host()
        {
            return _host;
        }

        public int Port()
        {
            return _port;
        }
    }

    public class PassiveDataConnection : IDataConnection
    {
        
        public PassiveDataConnection()
        {
            
        }

        public bool IsConnected()
        {
            return false;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public string Host()
        {
            throw new NotImplementedException();
        }

        public int Port()
        {
            throw new NotImplementedException();
        }

        public Stream Stream()
        {
            throw new NotImplementedException();
        }
    }
}