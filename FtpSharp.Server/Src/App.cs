﻿using System;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server
{
    class App
    {
        static bool sigintReceived = false;
        static void Main(string[] args)
        {
            ILogger logger = ApplicationLogging.CreateLogger<App>();

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
            
            try
            {
                var server = new Server(config);

                GracefulShutdown(logger, server);

                // bind ip address and port
                server.Bind();

                // bind succeed, start server
                server.Start();
            } catch (Exception e)
            {
                Console.WriteLine($"error start FtpSharp Server {e.Message}");
                Environment.Exit(-1);
            }
        }

        static void GracefulShutdown(ILogger logger, Server server)
        {
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) => {
                e.Cancel = true;

                // send signal to server process
                Server.isRunning = false;
                Server.acceptDone.Set();

                server.Shutdown();

                // signal sigint
                sigintReceived = true;
                logger.LogInformation("terminate FtpSharp Process");
            };

            AppDomain.CurrentDomain.ProcessExit += (object sender, EventArgs args) => {
                // send signal to server process
                if (!sigintReceived)
                {
                    Server.isRunning = false;
                    Server.acceptDone.Set();

                    server.Shutdown();
                    logger.LogInformation("terminate FtpSharp Process");
                }
            };
        }
    }
}
