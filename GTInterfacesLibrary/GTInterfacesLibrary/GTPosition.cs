using System;

namespace GTInterfacesLibrary
{
	public class GTPosition : IPosition
	{
		// Fields: 
		private int _x;
		private int _y;

		// Constructor: 
		public GTPosition(int x, int y)
		{
			_x = x;
			_y = y;
		}

		// Property implementation: 
		public int x
		{
			get
			{
				return _x;
			}

			set
			{
				_x = value;
			}
		}

		public int y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}
	}
}

