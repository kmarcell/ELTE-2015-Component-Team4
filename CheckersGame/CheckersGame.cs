using System;
using ConnectionInterface;
using ConnectionInterface.GameEvents;

namespace CheckersGame
{
    public class CheckersGame : IGame
    {
        private IPlatformGameManager PlatformGameManager;
        private GTInterfacesLibrary.GTGameLogicInterface<Logic.Element, Logic.Position> logic;

        public CheckersGame()
        {
            Name = "CheckersGame";
            Id = 2;
            Description = "CheckersGame";
            logic = new Logic.Logic("White");
        }

        public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEventArg;
        public void SendGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs)
        {
            // TODO: 
            // Raise the SendGameStateChangedEventArg event with the current game state.
            GameStateChangedEventArgs eventArgs = new GameStateChangedEventArgs();
            eventArgs.GameState = BytesFromGameState(logic.getCurrentState());
            SendGameStateChangedEventArg(this, eventArgs);
        }

        public void RegisterGameManager(IPlatformGameManager platformGameManager)
        {
            PlatformGameManager = platformGameManager;
            PlatformGameManager.SendGameStateChangedEvent += RecieveGameState;
        }

        public void RegisterArtificialIntelligence(IArtificialIntelligence artificialIntelligence)
        {
            throw new NotImplementedException();
        }

        public void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs)
        {
            // TODO:
            // Update game logic with the received game state.
            Logic.Step step = StepFromBytes(gameStateChangedEventArgs.GameState);
            logic.updateGameSpace(step);
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

        private Logic.Step StepFromBytes(byte[] state)
        {
            throw new NotImplementedException();
        }

        private byte[] BytesFromGameState(GTInterfacesLibrary.GTGameSpaceInterface<Logic.Element, Logic.Position> state)
        {
            throw new NotImplementedException();
        }
    }
}
