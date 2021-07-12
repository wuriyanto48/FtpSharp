using System;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server 
{
    public static class ApplicationLogging
    {
        private static ILoggerFactory _Factory = null;

        public static ILoggerFactory Factory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = LoggerFactory.Create(builder => {
                        builder
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                        .AddConsole();
                    });
                }
                return _Factory;
            }
            set { _Factory = value; }
        }
        public static ILogger CreateLogger<T>() => Factory.CreateLogger<T>();
    }
}