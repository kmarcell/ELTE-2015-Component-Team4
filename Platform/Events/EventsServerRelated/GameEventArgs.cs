using System;
using GTInterfacesLibrary.MessageTypes;

namespace Platform.Events.EventsServerRelated
{
    public class GameEventArgs : EventArgs
    {
        public Game Game { get; set; }
    }
}
