using System;
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
    }
}
