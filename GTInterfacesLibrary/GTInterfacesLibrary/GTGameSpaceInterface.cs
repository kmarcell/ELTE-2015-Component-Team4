using System;
using System.Collections.Generic;

namespace GTInterfacesLibrary
{
	public interface IPosition : IEquatable<IPosition>
	{
		int[] coordinates();
	}

	public interface GTGameSpaceInterface<E, P> : IEnumerable<KeyValuePair<P, E>> where E : GTGameSpaceElementInterface where P : IPosition
	{
		Boolean hasElementAt(P position);
		E elementAt(P position);
		void setElementAt(P position, E element);
		void mutateStateWith(GTGameStepInterface<E, P> step);
		GTGameSpaceInterface<E, P> stateWithStep(GTGameStepInterface<E, P> step);

        int nextPlayer
        {
            get;
        }
	}
}

