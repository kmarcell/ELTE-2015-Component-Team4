using System;
using GTInterfacesLibrary.GameEvents;

namespace GTInterfacesLibrary
{
    public interface GTGameLogicInterface<E, P> where E : GTGameSpaceElementInterface where P : IPosition
	{
		// Input
		void init();
		void updateGameSpace(GTGameStepInterface<E, P> step);
        // void addPlayer(GTPlayerInterface<E, P> player);

		// Output
		Boolean isGameOver();
		GTGameSpaceInterface<E, P> getCurrentState();
		GTGameStateGeneratorInterface<E, P> getStateGenerator();
		GTGameStateHashInterface<E, P> getStateHash();

		GTPlayerInterface<E, P> nextPlayer
		{
			get;
		}

		int gamePhase
		{
			get;
		}
	}
}

