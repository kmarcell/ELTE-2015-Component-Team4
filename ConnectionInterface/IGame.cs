using System;
using ConnectionInterface.GameEvents;

namespace ConnectionInterface
{
    public interface IGame
    {
        event EventHandler<GameStateChangedEventArgs> GameStateChanged;
        
        String Name { get; }

        Int32 Id { get; }

        String Description { get; }

        void LoadGame(Byte[] gameState);

        void ArtificialIntelligence(IArtificialIntelligence artificialIntelligence);

        Byte[] SaveGame();

        int GetHashCode();
    }
}
