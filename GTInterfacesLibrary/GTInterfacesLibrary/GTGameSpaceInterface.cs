using System;

namespace GTInterfacesLibrary
{
	public interface IPosition : IEquatable<IPosition>
	{
		int[] coordinates();
	}

	public interface GTGameSpaceInterface
	{
		Boolean hasElementAt(IPosition position);
		GTGameSpaceElementInterface elementAt(IPosition position);
		void setElementAt(IPosition position, GTGameSpaceElementInterface element);
		GTGameStepInterface differenceFromState(GTGameSpaceInterface previousState); // S = A - A' operator
		void mutateStateWith(GTGameStepInterface step); // A + S operator
	}
}

