using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace ArtificialIntelligence
{
    using TaskReturnType = GTGameSpaceInterface<GTGameSpaceElementInterface, IPosition>;
    public class CorrectAi : GTArtificialIntelligenceInterface<GTGameSpaceElementInterface, IPosition>, IGTArtificialIntelligenceInterface
    {

        async public Task<GTGameSpaceInterface<GTGameSpaceElementInterface, IPosition>>
            calculateNextStep(TaskReturnType gameSpace,
                              GTGameStateGeneratorInterface<GTGameSpaceElementInterface, IPosition> generator,
                              GTGameStateHashInterface<GTGameSpaceElementInterface, IPosition> hash)
        {
            TaskReturnType best= null;
            int bestValue = int.MinValue;
            GTPlayerInterface<GTGameSpaceElementInterface, IPosition> actualPlayer = gameSpace.getNextPlayer();

            Task<List<TaskReturnType>> listTask = generator.availableStatesFrom(gameSpace, gameSpace.getNextPlayer());

            await listTask;

            List<GTGameSpaceInterface<GTGameSpaceElementInterface, IPosition>> states = listTask.Result;

            foreach (TaskReturnType item in states)
            {
                int current = hash.evaluateState(item, actualPlayer);
                if (current > bestValue)
                {
                    best = item;
                    bestValue = current;
                }
            }

            return best;
        }

        public string Name
        {
            get { return "CorrectAi"; }
        }

        public string Description
        {
            get { return "A decent AI without any foresight"; }
        }

        public Difficulty Difficulty
        {
            get { return Difficulty.Normal; }
        }


    }
}
