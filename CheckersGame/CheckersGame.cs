using System;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;

namespace CheckersGame
{
    public class CheckersGame //: IGame
    {
        private GTPlatformManagerInterface PlatformGameManager;
        private GTInterfacesLibrary.GTGameLogicInterface<Logic.Element, Logic.Position> logic;
        private GTInterfacesLibrary.GTArtificialIntelligenceInterface<Logic.Element, Logic.Position> AI;
        private GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, IPosition> IAI;

        public CheckersGame()
        {
            Name = "CheckersGame";
            Id = 2;
            Description = "CheckersGame";
            logic = new Logic.Logic();
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

        public void RegisterGameManager(GTPlatformManagerInterface platformGameManager)
        {
            PlatformGameManager = platformGameManager;
            PlatformGameManager.SendGameStateChangedEvent += RecieveGameState;
        }

        public void RegisterArtificialIntelligence(GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, IPosition> artificialIntelligence)
        {
            IAI = artificialIntelligence;
        }

        public void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs)
        {
            // TODO:
            // Update game logic with the received game state.
            GameStateChangedEventArgs state = gameStateChangedEventArgs;
            if (state.GamePhase == GamePhase.Playing)
            {
                Logic.Step step = StepFromBytes(gameStateChangedEventArgs.GameState);
                logic.updateGameSpace(step);
            }            
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
