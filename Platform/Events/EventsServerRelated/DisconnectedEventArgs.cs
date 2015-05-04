using System;

namespace Platform.Events.EventsServerRelated
{
    public class DisconnectedEventArgs : EventArgs
    {
        public Boolean IsDisconnected { get; set; }
    }
}
