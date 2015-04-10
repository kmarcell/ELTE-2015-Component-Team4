using System;
using GTInterfacesLibrary;
using System.Collections.Generic;

namespace GTMillGameLogic
{
	public class GTMillGameStep : GTGameStepInterface<GTMillGameElement, GTMillPosition>
	{
		private GTMillPosition _from;
		private GTMillPosition _to;
		private GTMillGameElement _element;

		public GTGameStepInterface<GTMillGameElement, GTMillPosition> Create(GTMillGameElement element, GTMillPosition from, GTMillPosition to)
		{
			return new GTMillGameStep (element, from, to);
		}

		public GTMillGameStep(GTMillGameElement element, GTMillPosition from, GTMillPosition to)
		{
			this._from = from;
			this._to = to;
			this._element = element;
		}

		public GTMillPosition from
		{
			get
			{
				return _from;
			}
		}

		public GTMillPosition to
		{
			get
			{
				return _to;
			}
		}

		public GTMillGameElement element
		{
			get
			{
				return _element;
			}
		}
	}


}

