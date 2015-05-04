using System;
using System.Threading.Tasks;

namespace GTInterfacesLibrary
{
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

	public interface GTArtificialIntelligenceInterface<E, P> where E : GTGameSpaceElementInterface where P : IPosition
	{
		/**
		 *  Implement this function with async directive.
		 async*/ Task<GTGameStepInterface<E, P>> calculateNextStep(GTGameSpaceInterface<E, P> gameSpace, GTGameStateGeneratorInterface<E, P> generator, GTGameStateHashInterface<E, P> hash);

        String Name { get; }

        String Description { get; }
        
        Difficulty Difficulty { get; }
	}
}
