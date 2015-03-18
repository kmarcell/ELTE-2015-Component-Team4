using System;

namespace GTInterfacesLibrary
{
	delegate void GTHandleNextStepAction(GTGameStepInterface step);

	public interface GTArtificialIntelligenceInterface
	{
		GTGameStepInterface calculateNextStep(GTGameSpaceInterface gameSpace, GTGameStateGeneratorInterface generator, GTGameStateHashInterface hash);
		void calculateNextStepAsynchronously(GTGameSpaceInterface gameSpace, GTGameStateGeneratorInterface generator, GTGameStateHashInterface hash, GTHandleNextStepAction action);
	}
}
