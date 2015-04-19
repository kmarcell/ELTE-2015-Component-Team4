using System;
using ConnectionInterface.MessageTypes;

namespace Platform.EventsServerRelated
{
    public class GamesEventArgs : EventArgs
    {
        public Game[] Games { get; set; }
    }
}
