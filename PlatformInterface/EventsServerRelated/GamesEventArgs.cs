using System;
using ConnectionInterface.MessageTypes;

namespace PlatformInterface.EventsServerRelated
{
    public class GamesEventArgs : EventArgs
    {
        public Game[] Games { get; set; }
    }
}
