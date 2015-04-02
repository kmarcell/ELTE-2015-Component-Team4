using System;
using GTInterfacesLibrary;

namespace GTMillGameLogic
{
	public class GTMillPosition : IPosition
	{
		// Fields: 
		private int _x;
		private int _y;
		private int _z;

		// Constructor: 
		public GTMillPosition(int x, int y, int z)
		{
			_x = x;
			_y = y;
			_z = z;
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

		public int z
		{
			get
			{
				return _z;
			}
			set
			{
				_z = value;
			}
		}

		public int[] coordinates ()
		{
			return new int[]{ _x, _y, _z };
		}

		public override bool Equals(object obj) {
			return Equals (obj as GTPosition);
		}
		public override int GetHashCode ()
		{
			return 1000000 * z + 1000 * x + y;
		}

		public bool Equals(IPosition other)
		{
			int[] _coordinates = other.coordinates();
			return 
				_coordinates.Length == 3 &&
				this.x == _coordinates[0] &&
				this.y == _coordinates[1] &&
				this.z == _coordinates[2];
		}
	}
}

