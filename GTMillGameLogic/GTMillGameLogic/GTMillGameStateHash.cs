using System;
using GTInterfacesLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GTMillGameLogic
{
	public class GTMillGameStateHash : GTGameStateHashInterface<GTMillGameElement, GTMillPosition>
	{
        private GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state;

        private List<KeyValuePair<GTMillPosition, GTMillGameElement>> opponentElements;
        private List<KeyValuePair<GTMillPosition, GTMillGameElement>> ownElements;

        private HashSet<GTMillGameElement> horizontalMorrises;
        private HashSet<GTMillGameElement> verticalMorrises;
        private HashSet<GTMillGameElement> diagonalMorrises;

        private HashSet<GTMillGameElement> horizontal2Pieces;
        private HashSet<GTMillGameElement> vertical2Pieces;
        private HashSet<GTMillGameElement> diagonal2Pieces;

        int morrises;
        int twoPieces;
        int blockedOpponent;

        private void checkMorris(int direction, Tuple<List<GTMillGameElement>, List<GTMillGameElement>> neighbours)
        {
            if (neighbours.Item1.Count == 3 )
            {
                switch (direction)
                {
                    case 0:
                        if (!this.horizontalMorrises.Contains(neighbours.Item1.First()))
                        {
                            foreach (GTMillGameElement element in neighbours.Item1)
                            {
                                this.horizontalMorrises.Add(element);
                            }
                             
                            this.morrises++;
                        }

                        break;
                    case 1:
                        if (!this.verticalMorrises.Contains(neighbours.Item1.First()))
                        {
                            foreach (GTMillGameElement element in neighbours.Item1)
                            {
                                this.verticalMorrises.Add(element);
                            }

                            this.morrises++;
                        }

                        break;
                    case 2:
                        if (!this.diagonalMorrises.Contains(neighbours.Item1.First()))
                        {
                            foreach (GTMillGameElement element in neighbours.Item1)
                            {
                                this.diagonalMorrises.Add(element);
                            }

                            this.morrises++;
                        }

                        break;
                }
            }
        }

        private void check2Pieces(int direction, Tuple<List<GTMillGameElement>, List<GTMillGameElement>> neighbours)
        {
            if (neighbours.Item1.Count == 2 && neighbours.Item2.Count == 0)
            {
                switch (direction)
                {
                    case 0:
                        if (!this.horizontal2Pieces.Contains(neighbours.Item1.First()))
                        {
                            foreach (GTMillGameElement element in neighbours.Item1)
                            {
                                this.horizontal2Pieces.Add(element);
                            }

                            this.twoPieces++;
                        }

                        break;
                    case 1:
                        if (!this.vertical2Pieces.Contains(neighbours.Item1.First()))
                        {
                            foreach (GTMillGameElement element in neighbours.Item1)
                            {
                                this.vertical2Pieces.Add(element);
                            }

                            this.twoPieces++;
                        }

                        break;
                    case 2:
                        if (!this.diagonal2Pieces.Contains(neighbours.Item1.First()))
                        {
                            foreach (GTMillGameElement element in neighbours.Item1)
                            {
                                this.diagonal2Pieces.Add(element);
                            }

                            this.twoPieces++;
                        }

                        break;
                }
            }
        }

        private Tuple<List<GTMillGameElement>, List<GTMillGameElement>> getNeighbours(int direction, GTMillPosition position)
        {
            List<GTMillGameElement> opponentNeighbours = new List<GTMillGameElement>(),
                                    ownNeighbours = new List<GTMillGameElement>();

            switch (direction)
            {
                case 0:
                    for (int i = 0; i < 3; ++i)
                    {
                        GTMillPosition pos = new GTMillPosition(i, position.y, position.z);

                        if (state.hasElementAt(pos)) {
                            GTMillGameElement element = state.elementAt(pos);

                            if (element.owner == state.nextPlayer) {
                                ownNeighbours.Add(element);
                            } else {
                                opponentNeighbours.Add(element);
                            }
                        } 
                    }

                    break;
                case 1:
                    for (int i = 0; i < 3; ++i)
                    {
                        GTMillPosition pos = new GTMillPosition(position.x, i, position.z);

                        if (state.hasElementAt(pos)) {
                            GTMillGameElement element = state.elementAt(pos);

                            if (element.owner == state.nextPlayer) {
                                ownNeighbours.Add(element);
                            } else {
                                opponentNeighbours.Add(element);
                            }
                        } 
                    }
                        
                    break;
                case 2:
                    for (int i = 0; i < 3; ++i)
                    {
                        GTMillPosition pos = new GTMillPosition(position.x, position.y, i);

                        if (state.hasElementAt(pos)) {
                            GTMillGameElement element = state.elementAt(pos);

                            if (element.owner == state.nextPlayer) {
                                ownNeighbours.Add(element);
                            } else {
                                opponentNeighbours.Add(element);
                            }
                        } 
                    }

                    break;
            }

            Tuple<List<GTMillGameElement>, List<GTMillGameElement>> neighbours
                = new Tuple<List<GTMillGameElement>, List<GTMillGameElement>>(ownNeighbours, opponentNeighbours);

            return neighbours;
        }

        private bool isBlocked(int direction, GTMillPosition position, GTMillGameElement element)
        {
            bool blocked = true;

            switch (direction)
            {
                case 0:
                    //horizontal
                    if (position.x == 0 || position.x == 2)
                    {
                        GTMillPosition pos = new GTMillPosition(1, position.y, position.z);

                        blocked = blocked && state.hasElementAt(pos) && (state.elementAt(pos).owner != element.owner);
                    } else {
                        GTMillPosition pos1 = new GTMillPosition(0, position.y, position.z),
                                       pos2 = new GTMillPosition(2, position.y, position.z);

                        blocked = blocked && state.hasElementAt(pos1) && (state.elementAt(pos1).owner != element.owner);
                        blocked = blocked && state.hasElementAt(pos2) && (state.elementAt(pos2).owner != element.owner);
                    }

                    break;
                case 1:
                    //vertical
                    if (position.y == 0 || position.y == 2)
                    {
                        GTMillPosition pos = new GTMillPosition(position.x, 1, position.z);

                        blocked = blocked && state.hasElementAt(pos) && (state.elementAt(pos).owner != element.owner);
                    }
                    else
                    {
                        GTMillPosition pos1 = new GTMillPosition(position.x, 0, position.z),
                                       pos2 = new GTMillPosition(position.x, 2, position.z);

                        blocked = blocked && state.hasElementAt(pos1) && (state.elementAt(pos1).owner != element.owner);
                        blocked = blocked && state.hasElementAt(pos2) && (state.elementAt(pos2).owner != element.owner);
                    }

                    break;
                case 2:
                    //diagonal
                    if (position.z == 0 || position.z == 2)
                    {
                        GTMillPosition pos = new GTMillPosition(position.x, position.y, 1);

                        blocked = blocked && state.hasElementAt(pos) && (state.elementAt(pos).owner != element.owner);
                    }
                    else
                    {
                        GTMillPosition pos1 = new GTMillPosition(position.x, position.y, 0),
                                       pos2 = new GTMillPosition(position.x, position.y, 2);

                        blocked = blocked && state.hasElementAt(pos1) && (state.elementAt(pos1).owner != element.owner);
                        blocked = blocked && state.hasElementAt(pos2) && (state.elementAt(pos2).owner != element.owner);
                    }

                    break;
            }

            return blocked;
        }

        private void countPieces()
        {
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

                bool blocked = true;

                //Directions: 0 horizontal, 1 vertical, 2 diagonal
                for (int direction = 0; direction < 3; ++direction)
                {
                    //impossible to jump this direction
                    if (!(direction == 0 && element.Key.y == 1) && !(direction == 1 && element.Key.x == 1))
                    {
                        //if the next player has element at the position
                        if (element.Value.owner == state.nextPlayer)
                        {
                            Tuple<List<GTMillGameElement>, List<GTMillGameElement>> neighbours = this.getNeighbours(direction, element.Key);

                            //checking the morrises
                            this.checkMorris(direction, neighbours);
                            //checking 2 pieces configuration 
                            this.check2Pieces(direction, neighbours);
                        }
                        else
                        {
                            //checking blocking
                            blocked = blocked && this.isBlocked(direction, element.Key, element.Value);
                        }
                    }
                }

                if (blocked && element.Value.owner != state.nextPlayer)
                {
                    this.blockedOpponent++;
                }
            }
        }

        public int evaluateState(GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state)
        {
            this.state = state;

            this.morrises = 0;
            this.twoPieces = 0;
            this.blockedOpponent = 0;

            this.horizontalMorrises = new HashSet<GTMillGameElement>();
            this.verticalMorrises = new HashSet<GTMillGameElement>();
            this.diagonalMorrises = new HashSet<GTMillGameElement>();

            this.horizontal2Pieces = new HashSet<GTMillGameElement>();
            this.vertical2Pieces = new HashSet<GTMillGameElement>();
            this.diagonal2Pieces = new HashSet<GTMillGameElement>();

            this.opponentElements = new List<KeyValuePair<GTMillPosition, GTMillGameElement>>();
            this.ownElements = new List<KeyValuePair<GTMillPosition, GTMillGameElement>>();

            this.countPieces();

            throw new NotImplementedException();
        }
	}

}

