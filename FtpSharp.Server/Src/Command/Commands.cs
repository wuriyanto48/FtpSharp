using System;
using System.Collections.Generic;

namespace FtpSharp.Server.Command
{

    public sealed class Commands
    {
        private Dictionary<ECommand, ICommand> commands = new Dictionary<ECommand, ICommand>();

        public Commands(ClientObject clientObject)
        {
            commands.Add(ECommand.USER, new USERCommand(clientObject));
            commands.Add(ECommand.PASS, new PASSCommand(clientObject));

            commands.Add(ECommand.CWD, new CWDCommand(clientObject));
            commands.Add(ECommand.PWD, new PWDCommand(clientObject));
            commands.Add(ECommand.LIST, new LISTCommand(clientObject));

            commands.Add(ECommand.SIZE, new SIZECommand(clientObject));
            commands.Add(ECommand.MDTM, new MDTMCommand(clientObject));
            commands.Add(ECommand.RETR, new RETRCommand(clientObject));
            commands.Add(ECommand.STOR, new STORCommand(clientObject));

            commands.Add(ECommand.LPRT, new LPRTCommand(clientObject));
            commands.Add(ECommand.EPRT, new EPRTCommand(clientObject));
            commands.Add(ECommand.PORT, new PORTCommand(clientObject));

            commands.Add(ECommand.TYPE, new TYPECommand(clientObject));

            commands.Add(ECommand.QUIT, new QUITCommand(clientObject));
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
        EPRT,
        LPRT,
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
        MDTM,
        NLST,
        SITE,
        SYST,
        STAT,
        HELP,
        NOOP,
        SIZE,

        NOTVALID,
    }
}