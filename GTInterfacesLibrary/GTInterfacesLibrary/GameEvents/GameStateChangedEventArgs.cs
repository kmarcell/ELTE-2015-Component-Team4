using System;

namespace GTInterfacesLibrary.GameEvents
{
    /// <summary>
    /// The phase of the game, to perform the corresponding action.
    /// </summary>
    public enum GamePhase
    {
        Opened,
        Started,
        Playing,
        Ended
    }

    /// <summary>
    /// The type of the game, to handle all three possibility.
    /// </summary>
    public enum GameType
    {
        Online,
        Local,
        Ai
    }

    /// <summary>
    /// The event which is used for communication between platformamanger and gamelogic.
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

        /// <summary>
        /// Determine wether the player won or not.
        /// </summary>
        public bool IsWon { get; set; }

        /// <summary>
        /// The type of the game which should be set ad the beggining of the game.
        /// </summary>
        public GameType GameType { get; set; }
    }
}
