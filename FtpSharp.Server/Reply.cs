using System;
using System.Collections.Generic;

namespace FtpSharp.Server 
{
    public enum DataType
    {
        ASCII, BINARY
    }

    public static class Reply
    {
        public static string GetReply(int code)
        {
            Dictionary<int, string> replies = new Dictionary<int, string>();
            replies.Add(200, "200 Command okay.");
            replies.Add(500, "500 Syntax error, command unrecognized.");
            replies.Add(501, "501 Syntax error in parameters or arguments.");
            replies.Add(202, "202 Command not implemented, superfluous at this site.");
            replies.Add(502, "502 Command not implemented.");
            replies.Add(503  , "503 Bad sequence of commands.");
            replies.Add(504  , "504 Command not implemented for that parameter.");
            replies.Add(110  , "110 Restart marker reply.");
            replies.Add(211  , "211 System status, or system help reply.");
            replies.Add(212  , "212 Directory status.");
            replies.Add(13  , "213 File status.");
            replies.Add(214  , "214 Help message.");
            replies.Add(215  , "215 NAME system type.");
            replies.Add(120  , "120 Service ready in nnn minutes.");
            replies.Add(220  , "220 Service ready for new user.");
            replies.Add(221  , "221 Service closing control connection.");
            replies.Add(421  , "421 Service not available, closing control connection.");
            replies.Add(125  , "125 Data connection already open; transfer starting.");
            replies.Add(225  , "225 Data connection open; no transfer in progress.");
            replies.Add(425  , "425 Can't open data connection.");
            replies.Add(226  , "226 Closing data connection.");
            replies.Add(426  , "426 Connection closed; transfer aborted.");
            replies.Add(227  , "227 Entering Passive Mode (h1,h2,h3,h4,p1,p2).");
            replies.Add(230  , "230 User logged in, proceed.");
            replies.Add(530  , "530 Not logged in.");
            replies.Add(331  , "331 User name okay, need password.");
            replies.Add(332  , "332 Need account for login.");
            replies.Add(532  , "532 Need account for storing files.");
            replies.Add(150  , "150 File status okay; about to open data connection.");
            replies.Add(250  , "250 Requested file action okay, completed.");
            replies.Add(257  , "257 \"PATHNAME\" created.");
            replies.Add(350  , "350 Requested file action pending further information.");
            replies.Add(450  , "450 Requested file action not taken.");
            replies.Add(550  , "550 Requested action not taken.");
            replies.Add(451  , "451 Requested action aborted. Local error in processing.");
            replies.Add(551  , "551 Requested action aborted. Page type unknown.");
            replies.Add(452  , "452 Requested action not taken.");
            replies.Add(552  , "552 Requested file action aborted.");
            replies.Add(553  , "553 Requested action not taken.");

            var isExist = replies.TryGetValue(code, out var value);
            if (isExist)
            {
                return value;
            }

            return replies[502];
        }
    }
}