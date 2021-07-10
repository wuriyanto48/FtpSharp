using System;
using System.IO;

namespace FtpSharp.Server.Command
{
    public class PWDCommand : ICommand
    {
        private ClientObject _clientObject;

        public PWDCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
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