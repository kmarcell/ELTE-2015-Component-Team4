using System;
using GTInterfacesLibrary;
using System.Collections.Generic;

namespace GTMillGameLogic
{
	public class GTMillGameStep : GTGameStepInterface
	{
		protected Dictionary<GTMillPosition, int> _gameField;

		public GTMillGameStep()
		{
			this._gameField = new Dictionary<GTMillPosition, int>();
		}

		protected GTMillGameStep(Dictionary<GTMillPosition, int> gameField)
		{
			this._gameField = gameField;
		}

		public static GTMillGameStep operator +(GTMillGameStep a, GTMillGameStep b)
		{
			GTMillGameStep newStep = new GTMillGameStep (new Dictionary<GTMillPosition, int>(a._gameField));
			newStep.add(b);
			return newStep;
		}

		public static GTMillGameStep operator -(GTMillGameStep a, GTMillGameStep b)
		{
			GTMillGameStep newStep = new GTMillGameStep (new Dictionary<GTMillPosition, int>(a._gameField));
			newStep.substract(b);
			return newStep;
		}

		public void add (GTGameStepInterface other)
		{
			foreach (GTMillPosition p in (other as GTMillGameStep)._gameField.Keys) {
				_gameField [p] = _gameField [p] + (other as GTMillGameStep)._gameField [p];
				if (_gameField[p] == 0) {
					_gameField.Remove (p);
				}
			}
		}

		public void substract (GTGameStepInterface other)
		{
			throw new NotImplementedException ();
		}
	}


}

