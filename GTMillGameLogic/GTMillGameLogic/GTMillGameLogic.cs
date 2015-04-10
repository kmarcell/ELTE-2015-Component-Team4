using System;
using GTInterfacesLibrary;

namespace GTMillGameLogic
{
	public class GTMillGameLogic : GTGameLogicInterface<GTMillGameElement, GTMillPosition>
	{
		// properties
		private GTMillGameSpace _state = new GTMillGameSpace();

		public GTMillGameLogic ()
		{
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
	}
}

