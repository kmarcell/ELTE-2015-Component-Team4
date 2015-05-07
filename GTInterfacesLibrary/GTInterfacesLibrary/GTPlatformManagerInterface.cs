using System;
using GTInterfacesLibrary.GameEvents;

namespace GTInterfacesLibrary
{
    /// <summary>
    /// The interface of PlatformGameManager which should connect with the GameLogic <see cref="IGTGameLogicInterface"/>.
    /// </summary>
    public interface GTPlatformManagerInterface
    {
        /// <summary>
        /// The message which will send to the <see cref="IGTGameLogicInterface"/> if there is any change in game.
        /// </summary>
        event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEvent;

        /// <summary>
        /// The subscriber for event of <see cref="IGTGameLogicInterface"/>.
        /// </summary>
        /// <param name="sender">the sender which is <see cref="IGTGameLogicInterface"/>.</param>
        /// <param name="eventArgs">the game state represented eventargs with all fields</param>
        void RecieveGameStateFromLogic(object sender, GameStateChangedEventArgs eventArgs);
    }
}
