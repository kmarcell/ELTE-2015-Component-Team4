using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace CheckersGame.Logic
{
    using TaskReturnType = GTGameSpaceInterface<Element, Position>;
    public class CorrectAi : GTArtificialIntelligenceInterface<Element, Position>, IGTArtificialIntelligenceInterface
    {

        async public Task<GTGameSpaceInterface<Element, Position>>
            calculateNextStep(TaskReturnType gameSpace,
                              GTGameStateGeneratorInterface<Element, Position> generator,
                              GTGameStateHashInterface<Element, Position> hash)
        {

            TaskReturnType best = null;
            int bestValue = int.MinValue;
            GTPlayerInterface<Element, Position> actualPlayer = gameSpace.getNextPlayer();

            Task<List<TaskReturnType>> listTask = generator.availableStatesFrom(gameSpace, gameSpace.getNextPlayer());

            //await listTask;

            List<GTGameSpaceInterface<Element, Position>> states = listTask.Result;

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
