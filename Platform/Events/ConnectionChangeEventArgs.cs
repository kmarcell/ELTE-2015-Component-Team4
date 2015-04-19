using System;

namespace Platform.Events
{
    public class ConnectionChangeEventArgs : EventArgs
    {
        public Boolean IsConnected { get; set; }
    }
}
    