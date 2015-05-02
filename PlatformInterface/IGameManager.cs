using System;
using ConnectionInterface;
using ConnectionInterface.GameEvents;
using PlatformInterface.EventsGameRelated;
using PlatformInterface.EventsServerRelated;

namespace PlatformInterface
{
    public interface IGameManager
    {
        event EventHandler<EventArgs> GameStartedEvent;

        event EventHandler<GameEndedEventArgs> GameEndedEvent;

        void StartLocalGame(IArtificialIntelligence artificialIntelligence);

        void EndLocalGame();

        void SaveLocalGame(String fileName);

        void LoadLocalGame(String fileName);
        
        /// <summary>
        /// The function which register the loaded game in the GameManager.
        /// <remarks>
        /// We have to connect to SendGameStateChangedEvent event of IGame <see cref="GameStateChangedEventArgs"/>.
        /// </remarks>
        /// </summary>
        /// <param name="game"></param>
        void RegisterGame(IGame game);

        void RecieveGameStateFromNetwork(object sender, GameEventArgs eventArgs);
    }
}
