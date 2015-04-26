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
            Position[] positions = new Position[6];
            for (int i = 0; i < 6; i++)
            {
                int[] coordinates = position.coordinates();
                coordinates[i / 2] += (i % 2 == 0 ? -1 : 1);
                //positions[i] = new Position(coordinates[0], coordinates[1], coordinates[2]);
            }

            List<Position> availablePositions = new List<Position>();
            foreach (Position p in positions)
            {
                //if (!state.hasElementAt(p) && validPosition(p) && (position.z == p.z || !isCornerPosition(position)))
                {
                    availablePositions.Add(p);
                }
            }

            List<Step> steps = new List<Step>(availablePositions.Count);
            foreach (Position to in availablePositions)
            {
                //steps.Add(new Step(state.elementAt(position), position, to));
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
