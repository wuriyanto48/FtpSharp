﻿using System;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server
{
    class App
    {
        static void Main(string[] args)
        {
            ILogger logger = ApplicationLogging.CreateLogger<App>();


            if (args.Length <= 0)
            {
                logger.LogInformation("required config file");
                Environment.Exit(-1);
            }

            Config config = null;

            try
            {
                config = Config.FromFile(args[0]);
            } catch (Exception e)
            {
                logger.LogInformation($"error opening config file {e.Message}");
                Environment.Exit(-1);
            }
            
            try
            {
                using var server = new Server(config);

                // bind ip address and port
                server.Bind();

                // bind succeed, start server
                server.Start();
            } catch (Exception)
            {
                logger.LogInformation($"error start FtpSharp Server");
                Environment.Exit(-1);
            }
        }
    }
}
