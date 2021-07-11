using System;

namespace FtpSharp.Server
{
    public class QuitEventArgs : EventArgs
    {
        public QuitEventArgs(string sessionId)
        {
            SessionID = sessionId;
        }

        public string SessionID { get; set; }
    }

    public class QuitEventNotifier
    {
        public event EventHandler<QuitEventArgs> QuitCompletedEvent;

        public void Notify(QuitEventArgs args)
        {
            OnQuitCompletedEvent(args);
        }

        protected virtual void OnQuitCompletedEvent(QuitEventArgs args)
        {
            EventHandler<QuitEventArgs> completedEvent = QuitCompletedEvent;

            if (completedEvent != null)
            {
                completedEvent.Invoke(this, args);
            }
        }
    }

    public class QuitEventHandler
    {
        private Server _server;
        public void RegisterNotifier(QuitEventNotifier notifier, Server server)
        {
            Console.WriteLine("register notifier");
            _server = server;
            notifier.QuitCompletedEvent += HandleQuit;
        }

        private void HandleQuit(object sender, QuitEventArgs args)
        {
           bool removed = _server._clients.Remove(args.SessionID);
           Console.WriteLine($"remove {args.SessionID} : {removed}");

           _server.ShowClients();
        }

        public void UnregisterNotifier(QuitEventNotifier notifier)
        {
            notifier.QuitCompletedEvent -= HandleQuit;
        }
    }
}