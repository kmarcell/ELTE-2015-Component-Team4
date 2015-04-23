using System;
using ConnectionInterface;
using ConnectionInterface.GameEvents;

namespace CheckersGame
{
    public class CheckersGame : IGame
    {
        private IPlatformGameManager PlatformGameManager;

        public CheckersGame()
        {
            Name = "CheckersGame";
            Id = 2;
            Description = "CheckersGame";
        }

        public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEventArg;
        public void SendGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs)
        {
            throw new NotImplementedException();
        }

        public void RegisterGameManager(IPlatformGameManager platformGameManager)
        {
            PlatformGameManager = platformGameManager;
            PlatformGameManager.SendGameStateChangedEventArg += RecieveGameState;
        }

        public void RegisterArtificialIntelligence(IArtificialIntelligence artificialIntelligence)
        {
            throw new NotImplementedException();
        }

        public void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs)
        {
            throw new NotImplementedException();
        }

        public string Name { get; private set; }
        public int Id { get; private set; }
        public string Description { get; private set; }
        public void LoadGame(byte[] gameState)
        {
            throw new NotImplementedException();
        }

        public byte[] SaveGame()
        {
            throw new NotImplementedException();
        }
    }
}
