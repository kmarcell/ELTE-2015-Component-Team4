using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace GTMillGameLogic
{
    using TaskReturnType = GTGameSpaceInterface<GTMillGameElement, GTMillPosition>;
    public class AlphaBetaAi : GTArtificialIntelligenceInterface<GTMillGameElement, GTMillPosition>, IGTArtificialIntelligenceInterface
    {

        GTGameStateGeneratorInterface<GTMillGameElement, GTMillPosition> generatorFunction;
        GTGameStateHashInterface<GTMillGameElement, GTMillPosition> hashFunction;
        GTPlayerInterface<GTMillGameElement, GTMillPosition> maxPlayer;
        const int DEPTH = 4;

        public GTMillGameLogic _Logic;

        async public Task<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>>
            calculateNextStep(TaskReturnType gameSpace,
                              GTGameStateGeneratorInterface<GTMillGameElement, GTMillPosition> generator,
                              GTGameStateHashInterface<GTMillGameElement, GTMillPosition> hash)
        {
            generatorFunction = generator;
            hashFunction = hash;
            maxPlayer = _Logic.nextPlayer;
            //TODO: megnézni, hogy nyert-e már a lépő játékos


            TaskReturnType best = null;
            int bestValue = int.MinValue;

            Task<List<TaskReturnType>> listTask = generator.availableStatesFrom(gameSpace, _Logic.nextPlayer);

            await listTask;

            List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>> states = listTask.Result;


            if (states.Count == 0)
            {
                throw new Exception("There is no more state");
            }

            foreach (TaskReturnType item in states)
            {
                int current = calculateWithAlphaBeta(item, DEPTH, int.MinValue, int.MaxValue);
                if (current > bestValue)
                {
                    best = item;
                    bestValue = current;
                }
            }

            return best;
        }
        private int calculateWithAlphaBeta(TaskReturnType gameSpace, int depth, int alpha, int beta)
        {
            int v;

            if (depth == 0)
                return hashFunction.evaluateState(gameSpace, _Logic.nextPlayer);

            Task<List<TaskReturnType>> listTask = generatorFunction.availableStatesFrom(gameSpace, _Logic.nextPlayer);

            List<GTGameSpaceInterface<GTMillGameElement, GTMillPosition>> states = listTask.Result;

            if (states.Count == 0)
                return hashFunction.evaluateState(gameSpace, _Logic.nextPlayer);

            //TODO: lelőni az alfa bétát, ha nyert vagy vesztett valaki

            if (maxPlayer.id == _Logic.nextPlayer.id)
            {
                v = int.MinValue;
                foreach (TaskReturnType item in states)
                {
                    v = Math.Max(v, calculateWithAlphaBeta(item, depth - 1, alpha, beta));

                    alpha = Math.Max(v, alpha);

                    if (beta <= alpha)
                        break;
                }
                return v;
            }
            else
            {
                v = int.MaxValue;
                foreach (TaskReturnType item in states)
                {
                    v = Math.Min(v, calculateWithAlphaBeta(item, depth - 1, alpha, beta));

                    beta = Math.Min(v, beta);

                    if (beta <= alpha)
                        break;
                }
                return v;

            }
        }

        public string Name
        {
            get { return "AlphaBetaAi"; }
        }

        public string Description
        {
            get { return "This AI uses minmax algorithm with alpha beta pruning"; }
        }

        public Difficulty Difficulty
        {
            get { return Difficulty.Hard; }
        }

    }
}
