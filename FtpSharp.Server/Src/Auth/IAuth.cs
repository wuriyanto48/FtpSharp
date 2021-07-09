using System;

namespace FtpSharp.Server.Auth
{
    public interface IAuth
    {
        bool Check(string username, string password);
    }
}