using System;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using GameEndedEventArgs = Platform.Events.EventsGameRelated.GameEndedEventArgs;
using GameEventArgs = Platform.Events.EventsServerRelated.GameEventArgs;

namespace Platform.Model.Interface
{
    public interface IGameManager
    {
        event EventHandler<EventArgs> GameStartedEvent;

        event EventHandler<GameEndedEventArgs> GameEndedEvent;

        void StartLocalGame(IGTArtificialIntelligenceInterface artificialIntelligence);

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
        void RegisterGame(IGTGameLogicInterface game);

        void RecieveGameStateFromNetwork(object sender, GameEventArgs eventArgs);
    }
}
