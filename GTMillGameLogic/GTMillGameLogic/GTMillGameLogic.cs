using System;
using GTInterfacesLibrary;

namespace GTMillGameLogic
{
	public class GTMillGameLogic : GTGameLogicInterface
	{
		// properties
		private GTGameSpaceInterface _state = new GTMillGameSpace();

		public GTMillGameLogic ()
		{
		}

		// Input
		public void init()
		{
		}

		public void updateGameSpace(GTGameStepInterface step)
		{
			this._state.mutateStateWith (step);
		}

		// Output
		public Boolean isGameOver()
		{
			return false;
		}

		public GTGameSpaceInterface getCurrentState()
		{
			return _state;
		}

		public GTGameStateGeneratorInterface getStateGenerator()
		{
			return new GTMillGameStateGenerator();
		}

		public GTGameStateHashInterface getStateHash()
		{
			return new GTMillGameStateHash();
		}
	}
}

