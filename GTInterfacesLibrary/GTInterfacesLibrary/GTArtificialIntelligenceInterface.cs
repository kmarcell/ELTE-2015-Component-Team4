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

    public interface GTArtificialIntelligenceInterface<E, P> : IGTArtificialIntelligenceInterface
        where E : GTGameSpaceElementInterface 
        where P : IPosition
	{
		/**
		 *  Implement this function with async directive.
		 async*/
        Task<GTGameSpaceInterface<E, P>> calculateNextStep(GTGameSpaceInterface<E, P> gameSpace, GTGameStateGeneratorInterface<E, P> generator, GTGameStateHashInterface<E, P> hash);
	}

    public interface IGTArtificialIntelligenceInterface
    {
        String Name { get; }

        String Description { get; }

        Difficulty Difficulty { get; }
        
    }
}
