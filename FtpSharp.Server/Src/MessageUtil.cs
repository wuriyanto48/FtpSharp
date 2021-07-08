using System;
using System.Text;

namespace FtpSharp.Server
{
    public static class MessageUtil
    {
        public static readonly byte[] CarriageReturn = new byte[] {13, 10};
        public static readonly byte[] NewLine = new byte[] {10};

        public static byte[] EOL(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.ASCII:
                    return CarriageReturn;
                case DataType.BINARY:
                    return NewLine;
                default:
                    return NewLine;
            }
        }
        public static byte[] BuildReply(ClientObject clientObject, int code)
        {
            string reply = Reply.GetReply(code);
            byte[] byteData = Encoding.ASCII.GetBytes(reply);
            byteData = byteData.Concatenate(EOL(clientObject.dataType));
            return byteData;
        }

        public static byte[] BuildReply(ClientObject clientObject, int code, string message)
        {
            string reply = String.Format($"{code} {message}");
            byte[] byteData = Encoding.ASCII.GetBytes(reply);
            byteData = byteData.Concatenate(EOL(clientObject.dataType));
            return byteData;
        }
    }
}