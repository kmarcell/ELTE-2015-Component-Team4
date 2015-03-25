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

		public override bool Equals(object obj) {
			return Equals (obj as GTPosition);
		}
		public override int GetHashCode ()
		{
			return 1000 * x + y;
		}

		public bool Equals(IPosition other)
		{
			return this.x == other.x && this.y == other.y;
		}
	}
}

