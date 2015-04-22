using System;
using ConnectionInterface;
using ConnectionInterface.GameEvents;
using PlatformInterface.EventsGameRelated;

namespace PlatformInterface
{
    public interface IGameManager
    {
        event EventHandler<EventArgs> GameStartedEvent;

        event EventHandler<GameEndedEventArgs> GameEndedEvent;

        void StartGame();

        void EndGame();

        void SaveGame(String fileName);

        void LoadGame(String fileName);
        
        /// <summary>
        /// The function which register the loaded game in the GameManager.
        /// <remarks>
        /// We have to connect to SendGameStateChangedEventArg event of IGame <see cref="GameStateChangedEventArgs"/>.
        /// </remarks>
        /// </summary>
        /// <param name="game"></param>
        void RegisterGame(IGame game);

        event EventHandler<GameStateChangedEventArgs> SendNetworkGameStateChangedEventArg;

        void SendNetworkGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs);

        void RecieveNetworkGameState(object sender, GameStateChangedEventArgs eventArgs);
    }
}
