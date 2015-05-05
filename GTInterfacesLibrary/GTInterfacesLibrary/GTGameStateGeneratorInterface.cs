using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTInterfacesLibrary
{
	public interface GTGameStateGeneratorInterface<E, P> where E : GTGameSpaceElementInterface where P : IPosition
	{
        Task<List<GTGameSpaceInterface<E, P>>> availableStatesFrom(GTGameSpaceInterface<E, P> state);
	}
}
