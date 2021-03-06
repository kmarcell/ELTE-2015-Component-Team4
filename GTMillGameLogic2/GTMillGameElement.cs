using System;
using GTInterfacesLibrary;

namespace GTMillGameLogic
{
	public class GTMillGameElement : GTGameSpaceElementInterface
	{
		private int _id;
		private int _type;
		private int _owner;

		public GTMillGameElement(int id, int type, int owner)
		{
			if (id < 1) {
				throw new ArgumentException();
			}

			this.id = id;
			this.type = type;
			this.owner = owner;
		}

		public int id {
			get {
				return _id;
			}
			set {
				this._id = value;
			}
		}
		public int type {
			get {
				return _type;
			}
			set {
				this._type = value;
			}
		}
		public int owner {
			get {
				return _owner;
			}
			set {
				this._owner = value;
			}
		}
	}

}

