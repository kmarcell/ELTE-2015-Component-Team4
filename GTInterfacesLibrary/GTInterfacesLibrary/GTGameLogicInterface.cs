using System;

namespace GTInterfacesLibrary
{
	public interface GTGameLogicInterface<E, P> where E : GTGameSpaceElementInterface where P : IPosition
	{
		// Input
		void init();
		void updateGameSpace(GTGameStepInterface<E, P> step);

		// Output
		Boolean isGameOver();
		GTGameSpaceInterface<E, P> getCurrentState();
		GTGameStateGeneratorInterface<E, P> getStateGenerator();
		GTGameStateHashInterface<E, P> getStateHash();
	}
}

