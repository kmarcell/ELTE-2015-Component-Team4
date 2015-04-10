using System;
using GTInterfacesLibrary;
using System.Collections;
using System.Collections.Generic;

namespace GTMillGameLogic
{
	public class GTMillGameSpace : GTGameSpaceInterface<GTMillGameElement, GTMillPosition>, IEnumerable<KeyValuePair<GTMillPosition, GTMillGameElement>>
	{
		private Dictionary<GTMillPosition, GTMillGameElement> gameField;

		public GTMillGameSpace ()
		{
			this.gameField = new Dictionary<GTMillPosition, GTMillGameElement> ();
		}

		public Boolean hasElementAt (GTMillPosition position)
		{
			return this.gameField.ContainsKey(position);
		}

		public GTMillGameElement elementAt (GTMillPosition position)
		{
			return this.gameField [position];
		}

		public void setElementAt (GTMillPosition position, GTMillGameElement element)
		{
			this.gameField [position] = element;
		}

		public void mutateStateWith (GTGameStepInterface<GTMillGameElement, GTMillPosition> step)
		{
			if (step.from != GTMillPosition.Nowhere ()) {
				this.gameField.Remove (step.from);
			}

			if (step.to != GTMillPosition.Nowhere ()) {
				this.gameField [step.to] = step.element;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator ();
		}

		public IEnumerator<KeyValuePair<GTMillPosition, GTMillGameElement>> GetEnumerator()
		{
			return this.gameField.GetEnumerator ();
		}
	}

}

