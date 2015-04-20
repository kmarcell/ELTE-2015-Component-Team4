using System;

namespace ConnectionInterface.GameEvents
{
    public class GameStateChangedEventArgs : EventArgs
    {
        public Byte[] GameState { get; set; }
    }
}
