using System;

namespace FtpSharp.Server
{
    class App
    {
        static void Main(string[] args)
        {

            if (args.Length <= 0)
            {
                Console.WriteLine("required config file");
                Environment.Exit(-1);
            }

            Config config = null;

            try
            {
                config = Config.FromFile(args[0]);
            } catch (Exception e)
            {
                Console.WriteLine($"error opening config file {e.Message}");
                Environment.Exit(-1);
            }

            Console.WriteLine(config);
            
            using var server = new Server(config);
            server.Bind();
            server.Start();
        }
    }
}
