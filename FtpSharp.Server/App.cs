using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FtpSharp.Server
{
    class App
    {
        static void Main(string[] args)
        {
            Console.WriteLine(ECommand.ABOR.ToString());
            using var server = new Server("localhost", 8777);
            server.Bind();
            server.Start();
        }
    }
}
