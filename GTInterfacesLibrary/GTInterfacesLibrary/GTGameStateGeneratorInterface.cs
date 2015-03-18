using System;
using System.Threading.Tasks;

namespace GTInterfacesLibrary
{
	public interface GTGameStateGeneratorInterface
	{
		/**
		 *  Implement this function with async directive. - https://msdn.microsoft.com/en-us/library/hh191443.aspx
		 async*/ Task<GTGameSpaceInterface[]> availableStatesFrom(GTGameSpaceInterface state);
	}
}
