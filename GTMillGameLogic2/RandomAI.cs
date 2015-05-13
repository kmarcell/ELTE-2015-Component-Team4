using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace GTMillGameLogic
{
    using TaskReturnType = GTGameSpaceInterface<GTMillGameElement, GTMillPosition>;
    public class RandomAI : GTArtificialIntelligenceInterface<GTMillGameElement, GTMillPosition>, IGTArtificialIntelligenceInterface
    {

        public GTMillGameLogic _Logic;
       
        async public Task<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>>
            calculateNextStep(GTGameSpaceInterface<GTMillGameElement, GTMillPosition> gameSpace,
                              GTGameStateGeneratorInterface<GTMillGameElement, GTMillPosition> generator,
                              GTGameStateHashInterface<GTMillGameElement, GTMillPosition> hash)
        {

            Random rnd = new Random();

            Task<List<TaskReturnType>> listTask = generator.availableStatesFrom(gameSpace, _Logic.nextPlayer);

            await listTask;

            List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>> states = listTask.Result;

            if (states.Count == 0)
            {
                throw new Exception("There is no more state");
            }

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
