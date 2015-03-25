using System;
using GTInterfacesLibrary;
using System.Collections.Generic;

namespace GTMillGameLogic
{
	class GTMillGameSpace : GTMillGameStep, GTGameSpaceInterface
	{
		private Dictionary<int, GTGameSpaceElementInterface> elements;

		public GTMillGameSpace()
		{
			this.elements = new Dictionary<int, GTGameSpaceElementInterface> ();
		}

		public Boolean hasElementAt (IPosition position)
		{
			return this._gameField [position as GTPosition] != 0;
		}

		public GTGameSpaceElementInterface elementAt (IPosition position)
		{
			int id = this._gameField [position as GTPosition];
			if (id == 0) {
				return null;
			}
			return this.elements [id];
		}

		public void setElementAt (IPosition position, GTGameSpaceElementInterface element)
		{
			this.elements [element.id] = element;
			this._gameField [position as GTPosition] = element.id;
		}

		public GTGameStepInterface differenceFromState (GTGameSpaceInterface previousState)
		{
			return this - (previousState as GTMillGameSpace);
		}

		public void mutateStateWith (GTGameStepInterface step)
		{
			this.add (step);
			HashSet<int> elementIDs = new HashSet<int>(this._gameField.Values);
			foreach (int id in elements.Keys) {
				if (!elementIDs.Contains (id)) {
					// the element was removed from the game field
					elements.Remove(id);
				}
			}
		}
	}

}

