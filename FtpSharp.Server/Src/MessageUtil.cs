using System;
using System.Text;

namespace FtpSharp.Server
{
    public static class MessageUtil
    {
        public static readonly byte[] CRLF = new byte[] {13, 10};
        public static readonly byte[] LF = new byte[] {10};

        public static byte[] EOL(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.ASCII:
                    return CRLF;
                case DataType.BINARY:
                    return LF;
                default:
                    return LF;
            }
        }
        public static byte[] BuildReply(ClientObject clientObject, int code)
        {
            string reply = Reply.GetReply(code);
            byte[] byteData = Encoding.ASCII.GetBytes(reply);
            byteData = byteData.Concatenate(EOL(clientObject.DataType));
            return byteData;
        }

        public static byte[] BuildReply(ClientObject clientObject, int code, string message)
        {
            string reply = String.Format($"{code} {message}");
            byte[] byteData = Encoding.ASCII.GetBytes(reply);
            byteData = byteData.Concatenate(EOL(clientObject.DataType));
            return byteData;
        }

        public static string TrimCRLF(string input)
        {
            char[] charToTrim = {'\r', '\n'};
		    var s = input.Trim(charToTrim);
            return s;
        }
    }
}