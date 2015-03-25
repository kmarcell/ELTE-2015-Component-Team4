using System;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace GTMillGameLogic
{
	public class GTMillGameStateGenerator : GTGameStateGeneratorInterface
	{
		public async Task<GTGameSpaceInterface[]> availableStatesFrom (GTGameSpaceInterface state)
		{
			Task<GTGameSpaceInterface[]> t = Task.Factory.StartNew (() => {
				return new GTGameSpaceInterface[0];
			});

			GTGameSpaceInterface[] states = await t;
			return states;
		}
	}
}

