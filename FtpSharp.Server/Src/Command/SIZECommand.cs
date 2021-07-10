using System;
using System.IO;
using System.Text;

namespace FtpSharp.Server.Command
{
    public class SIZECommand : ICommand
    {
        private ClientObject _clientObject;

        public SIZECommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            Console.WriteLine("client send SIZE command");

            var targetPath = "";

            if (args.Length > 0)
            {
                var arg = args[0];
                arg = MessageUtil.TrimCRLF(arg);
                targetPath = Path.Join(_clientObject.RootDir, _clientObject.WorkDir, arg);
            } else
            {
                targetPath = Path.Join(_clientObject.RootDir, _clientObject.WorkDir);
            }

            FileInfo fileInfo = new FileInfo(targetPath);
            if (!fileInfo.Exists)
            {
                byte[] invalidCwdRequest = MessageUtil.BuildReply(_clientObject, 550, "File is not exist");
                _clientObject.Write(invalidCwdRequest);
                return;
            }

            // open file
            var size = fileInfo.Length;

            byte[] validListRequestData = MessageUtil.BuildReply(_clientObject, 213, size.ToString());
            _clientObject.Write(validListRequestData);
            
        }

        public bool ShouldLogin()
        {
            return true;
        }

    }
}