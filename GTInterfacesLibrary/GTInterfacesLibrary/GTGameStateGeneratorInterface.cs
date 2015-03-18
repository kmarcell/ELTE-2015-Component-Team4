using System;

namespace GTInterfacesLibrary
{
	public interface GTGameStateGeneratorInterface
	{
		GTGameSpaceInterface[] availableStatesFrom(GTGameSpaceInterface state);
	}
}

