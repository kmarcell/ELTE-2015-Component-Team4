using System;
using GTInterfacesLibrary;
using System.Collections;
using System.Collections.Generic;

namespace GTMillGameLogic
{
	public class GTMillGameStateHash : GTGameStateHashInterface<GTMillGameElement, GTMillPosition>
	{
        private GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state;

        private List<KeyValuePair<GTMillPosition, GTMillGameElement>> opponentElements;

        private List<KeyValuePair<GTMillPosition, GTMillGameElement>> ownElements;

        /*private int getClosedMorrises()
        {
            
            return 0;
        }*/

        private int getOpenedMorrises()
        {

            return 0;
        }

        /**
         * Number of morrises of one player
         **/
        private int getMorrises()
        {
            int piecesCount = 0;

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    Boolean isMillAtPos1 = true,
                            isMillAtPos2 = true,
                            isMillAtPos3 = true;

                    for (int k = 0; k < 3; ++k)
                    {
                        GTMillPosition pos1 = new GTMillPosition(i, j, k),
                                       pos2 = new GTMillPosition(i, k, j),
                                       pos3 = new GTMillPosition(k, i, j);

                        isMillAtPos1 = isMillAtPos1 && state.hasElementAt(pos1) && (state.elementAt(pos1).owner == state.nextPlayer);
                        isMillAtPos2 = isMillAtPos2 && state.hasElementAt(pos2) && (state.elementAt(pos2).owner == state.nextPlayer);
                        isMillAtPos3 = isMillAtPos3 && state.hasElementAt(pos3) && (state.elementAt(pos3).owner == state.nextPlayer);
                    }

                    if (isMillAtPos1) piecesCount++;
                    if (isMillAtPos2) piecesCount++;
                    if (isMillAtPos3) piecesCount++;
                }
            }

            return piecesCount;
        }

        private int getBlockedOpponentPieces()
        {

            return 0;
        }

        /**
         * Number of pieces of next player
         **/
        private int getOwnPieces()
        {
            return this.ownElements.Count;
        }

        /**
        * Number of pieces of the opponent player
        **/
        private int getOpponentPieces()
        {
            return this.opponentElements.Count;
        }

        private int get2Pieces()
        {
            int piecesCount = 0;

            for (int i = 0; i < 3; ++i)
            {
                
            }

            return piecesCount;
        }

        private int get3Pieces()
        {
            return 0;
        }

        public int evaluateState(GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state)
        {
            this.state = state;

            this.opponentElements = new List<KeyValuePair<GTMillPosition, GTMillGameElement>>();
            this.ownElements = new List<KeyValuePair<GTMillPosition, GTMillGameElement>>();

            foreach (KeyValuePair<GTMillPosition, GTMillGameElement> element in state)
            {
                if (element.Value.owner == state.nextPlayer)
                {
                    this.ownElements.Add(element);
                }
                else
                {
                    this.opponentElements.Add(element);
                }
            }

            throw new NotImplementedException();
        }
	}

}

