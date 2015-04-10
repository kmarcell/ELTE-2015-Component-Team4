using System;

namespace GTInterfacesLibrary
{
	public interface IPosition : IEquatable<IPosition>
	{
		int[] coordinates();
	}

	public interface GTGameSpaceInterface<E, P> where E : GTGameSpaceElementInterface where P : IPosition
	{
		Boolean hasElementAt(P position);
		E elementAt(P position);
		void setElementAt(P position, E element);
		void mutateStateWith(GTGameStepInterface<E, P> step); // A + S operator
	}
}

