
using System;

namespace FtpSharp.Server.Command
{
    public interface ICommand
    {
        void Process(string[] args);

        bool ShouldLogin();
    }
}