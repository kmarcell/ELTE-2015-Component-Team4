using System;

namespace PlatformInterface.EventsServerRelated
{
    public class ConnectionChangeEventArgs : EventArgs
    {
        public Boolean IsConnected { get; set; }
    }
}
    