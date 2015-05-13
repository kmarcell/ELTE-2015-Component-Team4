using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace GTMillGameLogic
{
    using TaskReturnType = GTGameSpaceInterface<GTMillGameElement, GTMillPosition>;
    public class CorrectAi : GTArtificialIntelligenceInterface<GTMillGameElement, GTMillPosition>, IGTArtificialIntelligenceInterface
    {

        public GTMillGameLogic _Logic;

        async public Task<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>>
            calculateNextStep(TaskReturnType gameSpace,
                              GTGameStateGeneratorInterface<GTMillGameElement, GTMillPosition> generator,
                              GTGameStateHashInterface<GTMillGameElement, GTMillPosition> hash)
        {

            TaskReturnType best = null;
            int bestValue = int.MinValue;
            GTPlayerInterface<GTMillGameElement, GTMillPosition> actualPlayer = _Logic.nextPlayer;

            Task<List<TaskReturnType>> listTask = generator.availableStatesFrom(gameSpace, actualPlayer);

            await listTask;

            List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>> states = listTask.Result;

            if (states.Count == 0)
            {
                throw new Exception("There is no more state");
            }
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
