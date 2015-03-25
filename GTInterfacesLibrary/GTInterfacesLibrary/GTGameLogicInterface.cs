using System;

namespace GTInterfacesLibrary
{
	public interface GTGameLogicInterface
	{
		// Input
		void init();
		void updateGameSpace(GTGameStepInterface gameSpace);

		// Output
		Boolean isGameOver();
		GTGameSpaceInterface getCurrentState();
		GTGameStateGeneratorInterface getStateGenerator();
		GTGameStateHashInterface getStateHash();
	}
}

