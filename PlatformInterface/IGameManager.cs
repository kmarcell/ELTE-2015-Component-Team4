using System;
using ConnectionInterface.MessageTypes;
using PlatformInterface.EventsGameRelated;

namespace PlatformInterface
{
    public interface IGameManager
    {
        event EventHandler<EventArgs> GameStartedEvent;

        event EventHandler<GameEndedEventArgs> GameEndedEvent;

        void StartGame(Game game);

        void EndGame();

        void SaveGame(String fileName);

        void LoadGame(String fileName);
    }
}
