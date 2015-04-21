using System;

namespace ConnectionInterface.GameEvents
{
    public enum GamePhase
    {
        Opened,
        Started,
        Playing,
        Ended
    }

    /// <summary>
    /// The event
    /// </summary>
    public class GameStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The byte representation of the game
        /// </summary>
        public Byte[] GameState { get; set; }

        /// <summary>
        /// The flag which is set to true if I have to step (server convert to false for the other user)
        /// <remarks>
        /// It is neccesseray to get all the step from logic and transfer, be visible for the other user, but not activate its table
        /// </remarks>
        /// </summary>
        public bool IsMyTurn { get; set; }

        /// <summary>
        /// The gamephase which is representing the current gamestate
        /// </summary>
        public GamePhase GamePhase { get; set; }


        public bool IsWon { get; set; }

        public bool IsOnline { get; set; }
    }
}
