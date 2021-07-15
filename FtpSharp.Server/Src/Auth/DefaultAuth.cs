using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace FtpSharp.Server.Auth
{
    public sealed class DefaultAuth : IAuth
    {
        private HMACSHA256 hMACSHA256;

        private Encoding _ascii = Encoding.ASCII;

        private string _serverUsername;
        
        private string _serverPassword;

        public DefaultAuth(string secret, string serverUsername, string serverPassword)
        {
             hMACSHA256 = new HMACSHA256(_ascii.GetBytes(secret));

            _serverUsername = serverUsername;
            _serverPassword = Convert.ToBase64String(hMACSHA256.ComputeHash(_ascii.GetBytes(serverPassword)));
        }

        public bool Check(string username, string password)
        {
            if (!EqualUsername(username))
                return false;

            if (!EqualPassword(password))
                return false;

            return true;
        }

        private bool EqualUsername(string username)
        {
            var usernameBytes = Encoding.ASCII.GetBytes(username);
            var serverUsernameBytes = Encoding.ASCII.GetBytes(_serverUsername);

            long diffUsername = usernameBytes.Length ^ serverUsernameBytes.Length;
            for (var i = 0; i < usernameBytes.Length && i < serverUsernameBytes.Length; i++)
                diffUsername |= (uint) usernameBytes[i] ^ (uint) serverUsernameBytes[i];

            if (diffUsername != 0)
                return false;
            
            return true;
        }

        private bool EqualPassword(string password)
        {
            var base64ReqPassword = Convert.ToBase64String(hMACSHA256.ComputeHash(_ascii.GetBytes(password)));

            var passwordBytes = Encoding.ASCII.GetBytes(base64ReqPassword);
            var serverPasswordBytes = Encoding.ASCII.GetBytes(_serverPassword);

            long diffPassword = passwordBytes.Length ^ serverPasswordBytes.Length;
            for (var i = 0; i < passwordBytes.Length && i < serverPasswordBytes.Length; i++)
                diffPassword |= (uint) passwordBytes[i] ^ (uint) serverPasswordBytes[i];

            if (diffPassword != 0)
                return false;

            return true;
        }
    }
}