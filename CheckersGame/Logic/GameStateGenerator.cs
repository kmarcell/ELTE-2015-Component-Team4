using System.Collections.Generic;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace CheckersGame.Logic
{
    using TaskReturnType = List<GTGameSpaceInterface<Element, Position>>;

    public class GameStateGenerator : GTGameStateGeneratorInterface<Element, Position>
    {
        public Task<TaskReturnType> availableStatesFrom(GTGameSpaceInterface<Element, Position> state)
        {
            Task<TaskReturnType> task = Task<TaskReturnType>.Factory.StartNew(() =>
            {
                List<Step> steps = new List<Step>();
                foreach (KeyValuePair<Position, Element> kv in state)
                {
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

        private bool validPosition(Position p)
        {
            if (p.x < 0 || p.x > 7 || p.y < 0 || p.y > 7)
                return false;

            if (BothEvenOrOdd(p.x, p.y))
                return false;

            return true;
        }

        private bool BothEvenOrOdd(int a, int b)
        {
            // Both number are even.
            if ((a % 2 == 0) && (b % 2 == 0))
                return true;

            // Both number are odd.
            if ((a % 2 != 0) && (b % 2 != 0))
                return true;

            return false;
        }

        private bool IsDame(Element e)
        {
            if (e.type == 0)
                return false;
            else
                return true;
        }

        public bool IsBlackCapture(GTGameSpaceInterface<Element, Position> state, Step s)
        {
            Position opp1pos = new Position(s.from.x + 1, s.to.y + 1);
            Position opp2pos = new Position(s.from.x + 1, s.to.y - 1);

            // NEG(Feketével akarunk ütni)
            if (state.elementAt(s.from).owner != 0)
                return false;

            // NEG(Előttünk van bábu és mögötte üres mező van)
            if (!state.hasElementAt(opp1pos) || state.hasElementAt(s.to))
                return false;

            // NEG(Az előttünk lévő bábu ellenfél)
            if (!AreOpponents(state.elementAt(opp1pos), state.elementAt(s.from)))
                return false;

            return true;
        }

        private bool AreOpponents(Element e1, Element e2)
        {
            return e1.owner != e2.owner;
        }




    }
}
