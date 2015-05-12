using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using GTInterfacesLibrary;

namespace CheckersGame.Logic
{
    using TaskReturnType = GTGameSpaceInterface<Element, Position>;
    public class RandomAI : GTArtificialIntelligenceInterface<Element, Position>
    {

        async public Task<GTGameSpaceInterface<Element, Position>>
            calculateNextStep(GTGameSpaceInterface<Element, Position> gameSpace,
                                GTGameStateGeneratorInterface<Element, Position> generator,
                                GTGameStateHashInterface<Element, Position> hash)
        {

            Random rnd = new Random();
            Task<List<TaskReturnType>> listTask = generator.availableStatesFrom(gameSpace, gameSpace.getNextPlayer());
            
            //await listTask;

            List<GTGameSpaceInterface<Element, Position>> states = listTask.Result;

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
