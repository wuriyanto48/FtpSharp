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
            var workDir = Path.Join(_clientObject.WorkDir, args[0]);
            var absolutePath = Path.GetFullPath(Path.Join(_clientObject.RootDir, workDir));

            Console.WriteLine($"curr dir 1: {Directory.GetCurrentDirectory()}");

            // Directory.SetCurrentDirectory(absolutePath);

            DirectoryInfo dir = new DirectoryInfo(workDir);

            Console.WriteLine($"curr dir 2: {Directory.GetCurrentDirectory()}");

            Console.WriteLine($" dir.Exists: {dir.Exists}");
            
            Console.WriteLine($"workDir: {Path.GetFullPath(workDir)}");
            Console.WriteLine($"absolutePath: {absolutePath}");

            Console.WriteLine("client send CWD command");
            Console.WriteLine($"{String.Join(",", args)}");

            _clientObject.WorkDir = workDir;
            
            byte[] data = MessageUtil.BuildReply(_clientObject, 200);

            _clientObject.SendMessage(data);
        }
    }
}