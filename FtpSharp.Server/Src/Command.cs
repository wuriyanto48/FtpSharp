using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;

namespace FtpSharp.Server 
{

    public interface ICommand
    {
        byte[] Process(string[] args);
    }

    public class Commands
    {
        private Dictionary<ECommand, ICommand> commands = new Dictionary<ECommand, ICommand>();

        public Commands(ClientObject clientObject)
        {
            commands.Add(ECommand.USER, new UserCommand(clientObject));
            commands.Add(ECommand.CWD, new CWDCommand(clientObject));
            commands.Add(ECommand.NOTVALID, new NotImplementedCommand(clientObject));
        }

        public ICommand GetCommand(ECommand eCommand)
        {
            var isExist = commands.TryGetValue(eCommand, out var value);
            if (isExist)
            {
                return value;
            }

            return commands[ECommand.NOTVALID];
        }
    }

    public class UserCommand : ICommand
    {
        private ClientObject _clientObject;

        public UserCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public byte[] Process(string[] args)
        {
            Console.WriteLine("client send USER command");
            Console.WriteLine($"{String.Join(",", args)}");
            return MessageUtil.SendReply(_clientObject, 230);
        }
    }

    public class CWDCommand : ICommand
    {
        private ClientObject _clientObject;

        public CWDCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public byte[] Process(string[] args)
        {
            var workDir = Path.Join(_clientObject.WorkDir, args[0]);
            var absolutePath = Path.Join(_clientObject.RootDir, workDir);
            
            Console.WriteLine($"absolutePath {absolutePath}");

            Console.WriteLine("client send CWD command");
            Console.WriteLine($"{String.Join(",", args)}");
            return MessageUtil.SendReply(_clientObject, 200);
        }
    }

    public class QuitCommand : ICommand
    {
        private ClientObject _clientObject;

        public QuitCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public byte[] Process(string[] args)
        {
            _clientObject._clientSocket.Shutdown(SocketShutdown.Both);  
            _clientObject._clientSocket.Close();
            return MessageUtil.SendReply(_clientObject, 221);
        }
    }

    public class NotImplementedCommand : ICommand
    {
        private ClientObject _clientObject;

        public NotImplementedCommand(ClientObject clientObject)
        {
            _clientObject = clientObject;
        }

        public byte[] Process(string[] args)
        {
            return MessageUtil.SendReply(_clientObject, 502);
        }
    }

    public enum ECommand
    {
        USER,
        PASS,
        ACCT,
        CWD,
        CDUP,
        SMNT,
        QUIT,
        REIN,
        PORT,
        PASV,
        TYPE,
        STRU,
        MODE,
        RETR,
        STOR,
        STOU,
        APPE,
        ALLO,
        REST,
        RNFR,
        RNTO,
        ABOR,
        DELE,
        RMD,
        MKD,
        PWD,
        LIST,
        NLST,
        SITE,
        SYST,
        STAT,
        HELP,
        NOOP,

        NOTVALID,
    }
}