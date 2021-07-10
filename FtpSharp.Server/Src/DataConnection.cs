using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;

namespace FtpSharp.Server 
{

    public interface IDataConnection
    {
        string Host();

        int Port();

        Stream Stream();

        TcpClient Client();

        void Close();
    }

    public class DefaultDataConnection : IDataConnection
    {
        private string _host { get; set; }

        private int _port { get; set; }

        public TcpClient TcpConn { get; }

        public DefaultDataConnection(string host, int port)
        {
            _host = host;
            _port = port;

            IPHostEntry ipHostInfo = Dns.GetHostEntry(_host);  
            IPAddress ipAddress = ipHostInfo.AddressList[0]; 
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, _port);
            TcpConn = new TcpClient();

            try
            {
                TcpConn.Connect(remoteEP);
            } catch(SocketException e)
            {
                throw new Exception(e.Message);
            }
        }

        public Stream Stream()
        {
            return TcpConn.GetStream();
        }

        public TcpClient Client()
        {
            return TcpConn;
        }

        public void Close()
        {
            if (TcpConn != null)
            {
                if (TcpConn.Connected)
                {
                    TcpConn.Close();
                }
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
}