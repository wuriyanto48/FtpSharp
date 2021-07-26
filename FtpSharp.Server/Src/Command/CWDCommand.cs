using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    public sealed class CWDCommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public CWDCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<CWDCommand>();
        }

        public void Process(string[] args)
        {
            var arg = args[0]; 
            MessageUtil.TrimCRLF(ref arg);

            var workDir = Path.Join(_clientObject.WorkDir, arg);
            var absolutePath = Path.Join(_clientObject.RootDir, workDir);

            _logger.LogInformation($"workDir: {Path.TrimEndingDirectorySeparator(workDir)}");
            _logger.LogInformation($"absolutePath: {absolutePath}");

            DirectoryInfo dir = new DirectoryInfo(absolutePath);
            if (!dir.Exists)
            {
                byte[] invalidCwdRequest = MessageUtil.BuildReply(_clientObject, 550, "Change directory failed");
                _clientObject.Write(invalidCwdRequest);
                return;
            }

            string[] files = Directory.GetFiles(absolutePath);
            foreach(var f in files)
            {
                _logger.LogInformation($"file {f}");
            }

            _logger.LogInformation($" dir.Exists: {dir.Exists}");

            _logger.LogInformation("client send CWD command");

            _clientObject.WorkDir = workDir;
            
            byte[] data = MessageUtil.BuildReply(_clientObject, 200);
            _clientObject.Write(data);
        }

        public bool ShouldLogin()
        {
            return true;
        }
    }
}