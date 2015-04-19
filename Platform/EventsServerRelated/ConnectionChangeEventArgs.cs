using System;

namespace Platform.EventsServerRelated
{
    public class ConnectionChangeEventArgs : EventArgs
    {
        public Boolean IsConnected { get; set; }
    }
}
    