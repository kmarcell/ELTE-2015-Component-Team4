using System;

namespace ConnectionInterface
{
    public interface IGame
    {
        String Name { get; }

        Int32 Id { get; }

        String Description { get; }

        void LoadGame(Byte[] gameState);

        Byte[] SaveGame();

        int GetHashCode();
    }
}
