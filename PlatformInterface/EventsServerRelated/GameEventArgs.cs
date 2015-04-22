using System;
using ConnectionInterface.MessageTypes;

namespace PlatformInterface.EventsServerRelated
{
    public class GameEventArgs : EventArgs
    {
        public Game Game { get; set; }
    }
}
