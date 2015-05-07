using System;

namespace GTInterfacesLibrary
{
	public interface GTGameStateHashInterface<E, P> where E : GTGameSpaceElementInterface where P : IPosition
	{
		int evaluateState(GTGameSpaceInterface<E, P> state, GTPlayerInterface<E, P> player);
	}
}
