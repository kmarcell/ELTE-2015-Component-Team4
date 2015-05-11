using System;
using GTInterfacesLibrary.GameEvents;

namespace GTInterfacesLibrary
{
    public interface GTGameInterface
    {
        /// <summary>
        /// The message which will send to the platform if there is any change in game
        /// </summary>
        event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEventArg;

        /// <summary>
        /// The function which will raise <see cref="GameStateChangedEventArgs"/> event.
        /// </summary>
        void SendGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs);

        /// <summary>
        /// The function where the platform can register its game manager after loading the dll <see cref="GTPlatformManagerInterface"/>.
        /// <remarks>
        /// In this function game has to connect to SendGameStateChangedEvent event of platform game manager <see cref="GameStateChangedEventArgs"/>.
        /// </remarks>
        /// </summary>
        /// <param name="platformGameManager"></param>
        void RegisterGameManager(GTPlatformManagerInterface platformGameManager);

        /// <summary>
        ///  The function where the platform can register the currently selected AI for the game.
        /// </summary>
        /// <param name="artificialIntelligenceName">The currently selected name of AI</param>
        void RegisterArtificialIntelligence(String artificialIntelligenceName);

        void RegisterGui(GTGuiInterface gui);

        /// <summary>
        /// The subscribe for event of platform game manager
        /// </summary>
        /// <param name="sender">the sender which is gamemanager of the platform</param>
        /// <param name="gameStateChangedEventArgs">the game state represented eventargs with all fields</param>
        void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs);

        /// <summary>
        /// The name of the game, to use in menu titles and in server list of online games
        /// </summary>
        String Name { get; }

        /// <summary>
        /// The id of the game, it should not be uniqe, but its better if we have different for games
        /// 1 - MillGame
        /// 2 - CheckersGame
        /// </summary>
        Int32 Id { get; }

        /// <summary>
        /// Short description of the game
        /// </summary>
        String Description { get; }

        /// <summary>
        /// Load game method, it is triggered by Platform if there is no game played
        /// </summary>
        /// <param name="gameState">the byte array representation of the game</param>
        void LoadGame(Byte[] gameState);

        /// <summary>
        /// The save method, it is triggered by Platform if there is any game in progress (not online)
        /// </summary>
        /// <returns>the byte array representation of the game</returns>
        Byte[] SaveGame();
    }
}
