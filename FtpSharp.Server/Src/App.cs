using System;
using System.IO;

namespace FtpSharp.Server
{
    class App
    {
        static void Main(string[] args)
        {
            var rootDir = Path.GetFullPath("public");
            
            using var server = new Server("localhost", 8777, rootDir);
            server.Bind();
            server.Start();
        }
    }
}
