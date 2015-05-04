using System;
using System.Linq;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;

namespace CheckersGame.Logic
{
    public class Logic: GTGameLogicInterface<Element, Position>
    {
        private GTPlatformManagerInterface PlatformGameManager;
        private GTArtificialIntelligenceInterface<Element, Position> AI;
        private GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, IPosition> IAI;

		public Logic()
        {
            Name = "CheckersGame";
            Id = 2;
            Description = "CheckersGame";
            init();
		}
        
        public string Name { get; private set; }
        public int Id { get; private set; }
        public string Description { get; private set; }

        // properties
        public GameSpace state = new GameSpace();

		// Input
		public void init()
		{
            StartingStateBuilder.BuildStartingState(state); 
		}

        public void updateGameSpace(GTGameStepInterface<Element, Position> step)
		{
			state.mutateStateWith(step);
		}

		// Output
		public Boolean isGameOver()
		{
            return state.Count() == 1;
		}

        public GTGameSpaceInterface<Element, Position> getCurrentState()
		{
			return state;
		}

		public GTGameStateGeneratorInterface<Element, Position> getStateGenerator()
		{
			return new GameStateGenerator();
		}

		public GTGameStateHashInterface<Element, Position> getStateHash()
		{
			return new GameStateHash();
		}

        public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEventArg;
        public void SendGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs)
        {
            // TODO: 
            // Raise the SendGameStateChangedEventArg event with the current game state.
            GameStateChangedEventArgs eventArgs = new GameStateChangedEventArgs();
            eventArgs.GameState = BytesFromGameState(getCurrentState());
            SendGameStateChangedEventArg(this, eventArgs);
        }

        public void RegisterGameManager(GTPlatformManagerInterface platformGameManager)
        {
            PlatformGameManager = platformGameManager;
            PlatformGameManager.SendGameStateChangedEvent += RecieveGameState;
        }

        public void RegisterArtificialIntelligence(IGTArtificialIntelligenceInterface artificialIntelligence)
        {
            IAI = (GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, IPosition>)artificialIntelligence;
        }

        public void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs)
        {
            // TODO:
            // Update game logic with the received game state.
            GameStateChangedEventArgs state = gameStateChangedEventArgs;
            if (state.GamePhase == GamePhase.Playing)
            {
                Step step = StepFromBytes(gameStateChangedEventArgs.GameState);
                updateGameSpace(step);
            }
        }
        public void LoadGame(byte[] gameState)
        {
            throw new NotImplementedException();
        }

        public byte[] SaveGame()
        {
            throw new NotImplementedException();
        }

        private Step StepFromBytes(byte[] state)
        {
            throw new NotImplementedException();
        }

        private byte[] BytesFromGameState(GTGameSpaceInterface<Element, Position> state)
        {
            throw new NotImplementedException();
        }
    }
}
