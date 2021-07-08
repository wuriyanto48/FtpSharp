using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FtpSharp.Server
{
    public class ClientObject
    {
        public const int BufferSize = 1024;

        public byte[] Buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();

        public Socket ClientSocket = null;

        public string RootDir;
        
        public string WorkDir;

    }
}