using System.Collections.Generic;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace CheckersGame.Logic
{
    using TaskReturnType = List<GTGameSpaceInterface<Element, Position>>;

    public class GameStateGenerator : GTGameStateGeneratorInterface<Element, Position>
    {
        public Task<TaskReturnType> availableStatesFrom(GTGameSpaceInterface<Element, Position> state, GTPlayerInterface<Element, Position> player)
        {
            Task<TaskReturnType> task = Task<TaskReturnType>.Factory.StartNew(() =>
            {
                List<Step> steps = new List<Step>();

                foreach (KeyValuePair<Position, Element> kv in state)
                {
                    if (kv.Value.owner == player.id)
                        steps.AddRange(stepsFromPositionWithState(state as GameSpace, kv.Key));
                }

                TaskReturnType states = new TaskReturnType();

                foreach (Step step in steps)
                {
                    states.Add(state.stateWithStep(step));
                }
                return states;
            });

            return task;
        }

        private List<Step> stepsFromPositionWithState(GameSpace state, Position position)
        {
            List<Step> steps = new List<Step>();

            for (int i = StepSupervisor.BoardMinIndex; i <= StepSupervisor.BoardMaxIndex; i++)
            {
                for (int j = StepSupervisor.BoardMinIndex; j <= StepSupervisor.BoardMaxIndex; j++)
                {
                    if (!state.hasElementAt(position))
                        continue;

                    Step step = new Step(state.elementAt(position), position, new Position(i, j));
                    if (!StepSupervisor.IsValidStep(state, step))
                        continue;
                    else
                        steps.Add(step);
                }
            }

            return steps;
        }
    }
}
