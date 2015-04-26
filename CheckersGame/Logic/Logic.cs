using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTInterfacesLibrary;

namespace CheckersGame.Logic
{
    public class Logic: GTGameLogicInterface<Element, Position>
    {
        // properties
        private GameSpace state = new GameSpace();

		public Logic ()
		{
            init();
		}

		// Input
		public void init()
		{
            PutUpBlacks();
            PutUpWhites();
		}

        public void updateGameSpace(GTGameStepInterface<Element, Position> step)
		{
			state.mutateStateWith(step);
		}

		// Output
		public Boolean isGameOver()
		{
            return state.Count() == 1;
		}

        public GTGameSpaceInterface<Element, Position> getCurrentState()
		{
			return state;
		}

		public GTGameStateGeneratorInterface<Element, Position> getStateGenerator()
		{
			return new GameStateGenerator();
		}

		public GTGameStateHashInterface<Element, Position> getStateHash()
		{
			return new GameStateHash();
		}

        private void PutUpBlacks()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 != 0)
                        {
                            Position p = new Position(i, j);
                            Element e = new Element(0, 0, 0);
                            state.setElementAt(p, e);
                        }
                    }
                    else
                    {
                        if (j % 2 == 0)
                        {
                            Position p = new Position(i, j);
                            Element e = new Element(0, 0, 0);
                            state.setElementAt(p, e);
                        }
                    }
                }
            }
        }

        private void PutUpWhites()
        {
            for (int i = 5; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 != 0)
                        {
                            Position p = new Position(i, j);
                            Element e = new Element(0, 0, 1);
                            state.setElementAt(p, e);
                        }
                    }
                    else
                    {
                        if (j % 2 == 0)
                        {
                            Position p = new Position(i, j);
                            Element e = new Element(0, 0, 1);
                            state.setElementAt(p, e);
                        }
                    }
                }
            }
        }
    }
}
