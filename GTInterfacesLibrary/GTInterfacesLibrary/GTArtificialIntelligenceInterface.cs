using System;
using System.Threading.Tasks;

namespace GTInterfacesLibrary
{
	public interface GTArtificialIntelligenceInterface
	{
		/**
		 *  Implement this function with async directive.
		 async*/ Task<GTGameStepInterface> calculateNextStep(GTGameSpaceInterface gameSpace, GTGameStateGeneratorInterface generator, GTGameStateHashInterface hash);
	}
}
