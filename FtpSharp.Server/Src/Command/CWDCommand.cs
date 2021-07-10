using System;
using System.IO;

namespace FtpSharp.Server.Command
{
    public class CWDCommand : ICommand
    {
        private ClientObject _clientObject;

        public CWDCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            var arg = MessageUtil.TrimCRLF(args[0]);
            var workDir = Path.Join(_clientObject.WorkDir, arg);
            var absolutePath = Path.Join(_clientObject.RootDir, workDir);

            Console.WriteLine($"workDir: {Path.TrimEndingDirectorySeparator(workDir)}");
            Console.WriteLine($"absolutePath: {absolutePath}");

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
                Console.WriteLine($"file {f}");
            }

            Console.WriteLine($" dir.Exists: {dir.Exists}");

            Console.WriteLine("client send CWD command");

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