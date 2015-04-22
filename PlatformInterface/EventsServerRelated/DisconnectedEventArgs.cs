using System;

namespace PlatformInterface.EventsServerRelated
{
    public class DisconnectedEventArgs : EventArgs
    {
        public Boolean IsDisconnected { get; set; }
    }
}
