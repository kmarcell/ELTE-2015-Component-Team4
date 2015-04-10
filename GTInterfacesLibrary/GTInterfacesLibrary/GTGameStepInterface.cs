using System;
using System.Collections.Generic;

namespace GTInterfacesLibrary
{
	public interface GTGameStepInterface<E, P> where E : GTGameSpaceElementInterface where P : IPosition
	{
		GTGameStepInterface<E, P> Create(E element, P from, P to);
		P from { get; }
		P to { get; }
		E element { get; }
	}
}
