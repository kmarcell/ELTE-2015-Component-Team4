using System;
using GTInterfacesLibrary;

namespace GTMillGameLogic
{
	public class GTMillGameStateHash : GTGameStateHashInterface<GTMillGameElement, GTMillPosition>
	{
        private GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state;

        private int getClosedMorrises()
        {

            return 0;
        }

        private int getOpenedMorrises()
        {

            return 0;
        }

        private int getMorrises()
        {

            return 0;
        }

        private int getBlockedOpponentPieces()
        {

            return 0;
        }

        private int getPieces()
        {

            return 0;
        }

        private int get2Pieces()
        {

            return 0;
        }

        private int get3Pieces()
        {

            return 0;
        }

        public int evaluateState(GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state)
        {
            this.state = state;

            throw new NotImplementedException();
        }
	}

}

