using System;
using GTInterfacesLibrary.MessageTypes;

namespace Platform.Events.EventsServerRelated
{
    public class GamesEventArgs : EventArgs
    {
        public Game[] Games { get; set; }
    }
}
