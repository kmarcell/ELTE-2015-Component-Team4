using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;

namespace CheckersGame.Logic
{
    public class Logic: GTGameLogicInterface<Element, Position>, IGTGameLogicInterface
    {
        private GTPlatformManagerInterface PlatformGameManager;
        private GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, GTPosition> AI;

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
            GameStateChangedEventArgs eventArgs = new GameStateChangedEventArgs();
            eventArgs.GamePhase = isGameOver() ? GamePhase.Ended : GamePhase.Playing;
            eventArgs.GameState = BytesFromGameState((GameSpace)getCurrentState());
            SendGameStateChangedEventArg(this, eventArgs);
        }

        public void RegisterGameManager(GTPlatformManagerInterface platformGameManager)
        {
            PlatformGameManager = platformGameManager;
            PlatformGameManager.SendGameStateChangedEvent += RecieveGameState;
        }

        public void RegisterArtificialIntelligence(IGTArtificialIntelligenceInterface artificialIntelligence)
        {
            AI = (GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, GTPosition>)artificialIntelligence;
        }

        public void RegisterGui(GTGuiInterface gui)
        {
            // register
        }

        public void RecieveGameState(object sender, GameStateChangedEventArgs gameStateChangedEventArgs)
        {
            if (gameStateChangedEventArgs.GamePhase == GamePhase.Playing)
            {
                GameSpace receivedState = StateFromBytes(gameStateChangedEventArgs.GameState);
                state = receivedState;
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

        private GameSpace StateFromBytes(byte[] bytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            GameSpace state = (GameSpace)binForm.Deserialize(memStream);
            return state;
        }

        private byte[] BytesFromGameState(GameSpace state)
        {
            if (state == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, state);
            return ms.ToArray();
        }
    }
}
