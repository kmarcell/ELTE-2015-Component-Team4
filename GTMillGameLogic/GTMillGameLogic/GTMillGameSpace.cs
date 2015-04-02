using System;
using GTInterfacesLibrary;
using System.Collections;
using System.Collections.Generic;

namespace GTMillGameLogic
{
	public class GTMillGameSpace : GTMillGameStep, GTGameSpaceInterface, IEnumerable<KeyValuePair<GTMillPosition, GTGameSpaceElementInterface>>
	{
		private Dictionary<int, GTGameSpaceElementInterface> elements;

		public GTMillGameSpace ()
		{
			this.elements = new Dictionary<int, GTGameSpaceElementInterface> ();
		}

		public Boolean hasElementAt (IPosition position)
		{
			int[] coord = position.coordinates ();
			if (coord.Length < 3) {
				throw new ArgumentException ();
			}
			return this._gameField [new GTMillPosition (coord [0], coord [1], coord [2])] != 0;
		}

		public GTGameSpaceElementInterface elementAt (IPosition position)
		{
			int[] coord = position.coordinates ();
			if (coord.Length < 3) {
				throw new ArgumentException ();
			}
			int id = this._gameField [new GTMillPosition (coord [0], coord [1], coord [2])];
			if (id == 0) {
				return null;
			}
			return this.elements [id];
		}

		public void setElementAt (IPosition position, GTGameSpaceElementInterface element)
		{
			int[] coord = position.coordinates ();
			if (coord.Length < 3) {
				throw new ArgumentException ();
			}
			this.elements [element.id] = element;
			this._gameField [new GTMillPosition (coord [0], coord [1], coord [2])] = element.id;
		}

		public GTGameStepInterface differenceFromState (GTGameSpaceInterface previousState)
		{
			return this - (previousState as GTMillGameSpace);
		}

		public void mutateStateWith (GTGameStepInterface step)
		{
			this.add (step);
			HashSet<int> elementIDs = new HashSet<int> (this._gameField.Values);
			foreach (int id in elements.Keys) {
				if (!elementIDs.Contains (id)) {
					// the element was removed from the game field
					elements.Remove (id);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator ();
		}

		public IEnumerator<KeyValuePair<GTMillPosition, GTGameSpaceElementInterface>> GetEnumerator ()
		{
			foreach (KeyValuePair<GTMillPosition, int> p in this._gameField) {
				yield return new KeyValuePair<GTMillPosition, GTGameSpaceElementInterface> (p.Key, this.elements [p.Value]);
			}
		}
	}

}

