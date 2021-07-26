using System;
using System.Text;

namespace FtpSharp.Server
{
    public enum DataType
    {
        ASCII, IMAGE, EBCDIC, DEFAULT
    }

    public static class MessageUtil
    {
        public static readonly byte[] CRLF = new byte[] {0xD, 0xA};

        public static readonly byte[] LF = new byte[] {0xA};

        // https://www.ibm.com/docs/en/xl-fortran-aix/16.1.0?topic=appendix-ascii-ebcdic-character-sets
        public static readonly byte[] NAK = new byte[] {0x15};

        public static byte[] EOL(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.ASCII:
                    return CRLF;
                case DataType.IMAGE:
                    return LF;
                case DataType.EBCDIC:
                    return NAK;
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
            var s = input.Trim(Encoding.ASCII.GetChars(CRLF));
            return s;
        }
    }
}