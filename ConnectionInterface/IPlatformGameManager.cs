using System;
using ConnectionInterface.GameEvents;

namespace ConnectionInterface
{
    public interface IPlatformGameManager
    {
        /// <summary>
        /// The message which will send to the gamelogic if there is any change in game
        /// </summary>
        event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEventArg;

        /// <summary>
        /// The function which will raise <see cref="GameStateChangedEventArgs"/> event.
        /// </summary>
        void SendGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs);

        /// <summary>
        /// The subscribe for event of game logic
        /// </summary>
        /// <param name="sender">the sender which is game logic</param>
        /// <param name="eventArgs">the game state represented eventargs with all fields</param>
        void RecieveGameState(object sender, GameStateChangedEventArgs eventArgs);
    }
}
