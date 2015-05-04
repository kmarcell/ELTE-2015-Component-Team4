using System;

namespace Platform.Events.EventsServerRelated
{
    public class ConnectionChangeEventArgs : EventArgs
    {
        public Boolean IsConnected { get; set; }
    }
}
    