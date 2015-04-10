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

		// Helpers
		public Boolean canMoveFromPositionToPosition(GTMillPosition from, GTMillPosition to)
		{
			// check if coordinates are valid
			if (from.x < 0 || from.y < 0 || from.z < 0 || to.x < 0 || to.y < 0 || to.z < 0) {
				return false;
			}

			// check if To position is occupied
			if (this._state.hasElementAt(to)) {
				return false;
			}

			// check if same middle point on different levels
			Boolean isSamePoints = from.x == to.x && from.y == to.y;
			Boolean isFromMiddle = Math.Abs (from.x - from.y) == 1;
			Boolean isOnDifferentLevel = Math.Abs (to.z - from.z) == 1;
			int distance = 0;//(int)Math.Sqrt (Math.Pow (from.x - from.y, 2) + Math.Pow (to.x - to.y, 2));

			if (isSamePoints && isFromMiddle) {
				return isOnDifferentLevel;
			}

			if (distance != 1 || isOnDifferentLevel) {
				return false;
			}

			return true;
		}
	}
}

