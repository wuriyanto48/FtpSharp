using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FtpSharp.Server.Command
{
    public sealed class STORCommand : ICommand
    {
        private ClientObject _clientObject;

        private readonly ILogger _logger;

        public STORCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
            _logger = ApplicationLogging.CreateLogger<STORCommand>();
        }

        public void Process(string[] args)
        {
            _logger.LogInformation("client send STOR command");

            var arg = args[0];
            arg = MessageUtil.TrimCRLF(arg);
            var targetPath = Path.Join(_clientObject.RootDir, _clientObject.WorkDir, arg);

            // FileInfo fileInfo = new FileInfo(targetPath);
            // if (!fileInfo.Exists)
            // {
            //     byte[] invalidCwdRequest = MessageUtil.BuildReply(_clientObject, 550, "File is not exist");
            //     _clientObject.Write(invalidCwdRequest);
            //     return;
            // }

            byte[] openingConnData = MessageUtil.BuildReply(_clientObject, 150);
            _clientObject.Write(openingConnData);

            // open file, if doesn't exit then create
            using FileStream fileStream = File.OpenWrite(targetPath);

            if (_clientObject.DataConn.Client() != null)
            {
                if (_clientObject.DataConn.Client().Connected)
                {
                    _clientObject.DataConn.Client().GetStream().CopyTo(fileStream);
                    _clientObject.DataConn.Close();
                    _clientObject.DataConn = null;
                }
            }

            byte[] validListRequestData = MessageUtil.BuildReply(_clientObject, 226);
            _clientObject.Write(validListRequestData);
            
        }

        public bool ShouldLogin()
        {
            return true;
        }

    }
}