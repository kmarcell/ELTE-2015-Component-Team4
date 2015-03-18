using System;

namespace GTInterfacesLibrary
{
	public interface IPosition
	{
		int x
		{
			get;
			set;
		}

		int y
		{
			get;
			set;
		}
	}

	public interface GTGameSpaceInterface
	{
		GTGameSpaceElementInterface elementAt(IPosition position);
		void setElementAt(IPosition position, GTGameSpaceElementInterface element);
	}
}

