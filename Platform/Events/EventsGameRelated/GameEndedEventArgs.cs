using System;

namespace Platform.Events.EventsGameRelated
{
    public class GameEndedEventArgs : EventArgs
    {
        public Boolean IsEnded { get; set; }

        public Boolean IsWin { get; set; }
    }
}
