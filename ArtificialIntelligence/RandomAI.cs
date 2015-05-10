using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace ArtificialIntelligence
{
    using TaskReturnType = GTGameSpaceInterface<GTGameSpaceElementInterface, IPosition>;
    public class RandomAI : GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, IPosition>, IGTArtificialIntelligenceInterface
    {
       
        async public Task<GTGameSpaceInterface<GTGameSpaceElementInterface, IPosition>>
            calculateNextStep(GTGameSpaceInterface<GTGameSpaceElementInterface, IPosition> gameSpace,
                              GTGameStateGeneratorInterface<GTGameSpaceElementInterface, IPosition> generator,
                              GTGameStateHashInterface<GTGameSpaceElementInterface, IPosition> hash)
        {

            Random rnd = new Random();

            Task<List<TaskReturnType>> listTask = generator.availableStatesFrom(gameSpace, gameSpace.getNextPlayer());

            await listTask;

            List<GTGameSpaceInterface<GTGameSpaceElementInterface, IPosition>> states = listTask.Result;

            return states[rnd.Next(states.Count)];
        }

        public string Name
        {
            get { return "RandomAi"; }
        }

        public string Description
        {
            get { return "You can't predict this crazy lunatic AI!"; }
        }

        public Difficulty Difficulty
        {
            get { return Difficulty.Easy; }
        }

        
    }
}
