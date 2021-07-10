using System;
using System.IO;
using System.Text;

namespace FtpSharp.Server.Command
{
    public class RETRCommand : ICommand
    {
        private ClientObject _clientObject;

        public RETRCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public void Process(string[] args)
        {
            Console.WriteLine("client send RETR command");

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

            byte[] openingConnData = MessageUtil.BuildReply(_clientObject, 150);
            _clientObject.Write(openingConnData);

            // open file
            FileStream fileStream = fileInfo.OpenRead();

            if (_clientObject.DataConn.Client() != null)
            {
                if (_clientObject.DataConn.Client().Connected)
                {
                    fileStream.CopyTo(_clientObject.DataConn.Client().GetStream());
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

        private string FormatFileInfo(string[] files)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--------------- File Info ----------------");
            sb.Append("\r\n");
            
            foreach(var f in files)
            {
                var pathParts = f.Split("/");
                var lastPath = pathParts[pathParts.Length - 1];
                sb.Append(lastPath);
                sb.Append("\r\n");
            }

            sb.Append("------------------------------------------");
            sb.Append("\r\n");

            return sb.ToString();
        }
    }
}