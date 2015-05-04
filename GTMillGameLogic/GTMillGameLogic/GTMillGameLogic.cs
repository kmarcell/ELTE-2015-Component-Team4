using System;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;

namespace GTMillGameLogic
{
	public class GTMillGameLogic : GTGameLogicInterface<GTMillGameElement, GTMillPosition>
	{
		// properties
		private GTMillGameSpace _state = new GTMillGameSpace();

		public GTMillGameLogic ()
        {
            Name = "MillGame";
            Id = 1;
            Description = "MillGame";
		}

		// Input
		public void init()
		{
		}

		public void updateGameSpace (GTGameStepInterface<GTMillGameElement, GTMillPosition> step)
		{
			this._state.mutateStateWith (step);
		}

		// Output
		public Boolean isGameOver()
		{
			return false;
		}

		public GTGameSpaceInterface<GTMillGameElement, GTMillPosition> getCurrentState()
		{
			return _state;
		}

		public GTGameStateGeneratorInterface<GTMillGameElement, GTMillPosition> getStateGenerator()
		{
			return new GTMillGameStateGenerator();
		}

		public GTGameStateHashInterface<GTMillGameElement, GTMillPosition> getStateHash()
		{
			return new GTMillGameStateHash();
		}

	    public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEventArg;
	    public void SendGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs)
	    {
	        throw new NotImplementedException();
	    }

	    public void RegisterGameManager(GTPlatformManagerInterface platformGameManager)
	    {
	        throw new NotImplementedException();
	    }

        public void RegisterArtificialIntelligence(IGTArtificialIntelligenceInterface artificialIntelligence)
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

