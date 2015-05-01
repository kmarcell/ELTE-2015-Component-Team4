using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersGame.Logic
{
    public static class StartingStateBuilder
    {
        private static GameSpace StartingState;

        public static void BuildStartingState(GameSpace state)
        {
            StartingState = state;
            PutUpBlacks();
            PutUpWhites();
        }

        private static void PutUpBlacks()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    PutUpBlackIfStartPosition(i, j);
                }
            }
        }

        private static void PutUpBlackIfStartPosition(int i, int j)
        {
            if (i % 2 == 0)
            {
                if (j % 2 != 0)
                {
                    Position p = new Position(i, j);
                    Element e = new Element(0, 0, 0);
                    StartingState.setElementAt(p, e);
                }
            }
            else
            {
                if (j % 2 == 0)
                {
                    Position p = new Position(i, j);
                    Element e = new Element(0, 0, 0);
                    StartingState.setElementAt(p, e);
                }
            }
        }

        private static void PutUpWhites()
        {
            for (int i = 5; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    PutUpWhiteIfStartPosition(i, j);
                }
            }
        }

        private static void PutUpWhiteIfStartPosition(int i, int j)
        {
            if (i % 2 == 0)
            {
                if (j % 2 != 0)
                {
                    Position p = new Position(i, j);
                    Element e = new Element(0, 0, 1);
                    StartingState.setElementAt(p, e);
                }
            }
            else
            {
                if (j % 2 == 0)
                {
                    Position p = new Position(i, j);
                    Element e = new Element(0, 0, 1);
                    StartingState.setElementAt(p, e);
                }
            }
        }
    }
}
