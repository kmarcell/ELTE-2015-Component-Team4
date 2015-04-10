using System;
using System.Threading.Tasks;

namespace GTInterfacesLibrary
{
	public interface GTGameStateGeneratorInterface<E, P> where E : GTGameSpaceElementInterface where P : IPosition
	{
		/**
		 *  Implement this function with async directive. - https://msdn.microsoft.com/en-us/library/hh191443.aspx
		 async*/ Task<GTGameSpaceInterface<E, P>[]> availableStatesFrom(GTGameSpaceInterface<E, P> state);
	}
}
