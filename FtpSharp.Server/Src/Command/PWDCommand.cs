using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    public sealed class PWDCommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public PWDCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<PWDCommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send PWD command");

            var currentDirectory = Path.Join(_clientObject.RootDir, _clientObject.WorkDir);
            byte[] data = MessageUtil.BuildReply(_clientObject, 257, $"\"{Path.GetFullPath(currentDirectory)}\"");
            _clientObject.Write(data);
        }

        public bool ShouldLogin()
        {
            return true;
        }
    }
}