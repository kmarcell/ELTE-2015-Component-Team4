using System;

namespace GTInterfacesLibrary.GameEvents
{
    /// <summary>
    /// The phase of the game, to perform the corresponding action.
    /// </summary>
    public enum GamePhase
    {
        /// <summary>
        /// Opened is used when the game is created online but no user joined.
        /// </summary>
        Opened,
        /// <summary>
        /// Started is used when the game is started.
        /// </summary>
        Started,
        /// <summary>
        /// Playing is used when the game is started but not finished.
        /// </summary>
        Playing,
        /// <summary>
        /// Ended is used when the game is finished due to win.
        /// </summary>
        Ended,
        /// <summary>
        /// Cancel is used when the game is cancelled.
        /// </summary>
        Cancelled
    }

    /// <summary>
    /// The type of the game, to handle all three possibility.
    /// </summary>
    public enum GameType
    {
        /// <summary>
        /// The Online GameType is used when the user playing against another user.
        /// </summary>
        Online,
        /// <summary>
        /// The local GameType is used when the user playing locally against an AI.
        /// </summary>
        Local,
        /// <summary>
        /// The AI GameTyoe is used then AI is playing against an AI.
        /// </summary>
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
