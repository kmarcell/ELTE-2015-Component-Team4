using System;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace GTMillGameLogic
{
	public class GTMillGameStateGenerator : GTGameStateGeneratorInterface<GTMillGameElement, GTMillPosition>
	{
		public async Task<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>[]> availableStatesFrom (GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state)
		{
			Task<GTMillGameSpace[]> t = Task.Factory.StartNew (() => {
				return new GTMillGameSpace[0];
			});

			GTMillGameSpace[] states = await t;
			return states;
		}
	}
}

