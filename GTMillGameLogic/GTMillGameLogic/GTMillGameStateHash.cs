using System;
using GTInterfacesLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Math;


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

        /**
         * True if we can jump in the given direction from the given position
         **/
        private bool isValidDirection(int direction, GTMillPosition position)
        {
            return !(direction == 0 && position.y == 1) //you cant jump horizontal
                    && !(direction == 1 && position.x == 1) //you cant jump vertical
                    && !(direction == 2 && position.x == 1 && position.y == 1); //we can only jump diagonal from there
        }

        /**
         * Returns the neighbour elements of the given position. 
         * The opponent and own elements will be seperated.
         **/
        private Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>> getNeighbours(int direction, GTMillPosition position)
        {
            List<Tuple<GTMillGameElement, bool>> opponentNeighbours = new List<Tuple<GTMillGameElement, bool>>(),
                                                 ownNeighbours = new List<Tuple<GTMillGameElement, bool>>();

            if (this.isValidDirection(direction, position))
            {
                switch (direction)
                {
                    case 0:
                        for (int i = 0; i < 3; ++i)
                        {
                            GTMillPosition pos = new GTMillPosition(i, position.y, position.z);

                            if (state.hasElementAt(pos))
                            {
                                GTMillGameElement element = state.elementAt(pos);
                                bool direct = Math.Abs(i - position.x) == 1;

                                if (element.owner == state.nextPlayer)
                                {
                                    ownNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                                else
                                {
                                    opponentNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                            }
                        }

                        break;
                    case 1:
                        for (int i = 0; i < 3; ++i)
                        {
                            GTMillPosition pos = new GTMillPosition(position.x, i, position.z);

                            if (state.hasElementAt(pos))
                            {
                                GTMillGameElement element = state.elementAt(pos);
                                bool direct = Math.Abs(i - position.y) == 1;

                                if (element.owner == state.nextPlayer)
                                {
                                    ownNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                                else
                                {
                                    opponentNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                            }
                        }

                        break;
                    case 2:
                        for (int i = 0; i < 3; ++i)
                        {
                            GTMillPosition pos = new GTMillPosition(position.x, position.y, i);

                            if (state.hasElementAt(pos))
                            {
                                GTMillGameElement element = state.elementAt(pos);
                                bool direct = Math.Abs(i - position.z) == 1;

                                if (element.owner == state.nextPlayer)
                                {
                                    ownNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                                else
                                {
                                    opponentNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                            }
                        }

                        break;
                }
            }

            Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>> neighbours
                = new Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>>(ownNeighbours, opponentNeighbours);

            return neighbours;
        }

        /**
         * Checks morrises in one direction
         **/
        private void checkMorris(int direction, Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>> neighbours)
        {
            //there is a morris
            if (neighbours.Item1.Count == 3 )
            {
                switch (direction)
                {
                    case 0:
                        if (!this.horizontalMorrises.Contains(neighbours.Item1.First().Item1))
                        {
                            foreach (Tuple<GTMillGameElement, bool> element in neighbours.Item1)
                            {
                                this.horizontalMorrises.Add(element.Item1);
                            }
                             
                            this.morrises++;
                        }

                        break;
                    case 1:
                        if (!this.verticalMorrises.Contains(neighbours.Item1.First().Item1))
                        {
                            foreach (Tuple<GTMillGameElement, bool> element in neighbours.Item1)
                            {
                                this.verticalMorrises.Add(element.Item1);
                            }

                            this.morrises++;
                        }

                        break;
                    case 2:
                        if (!this.diagonalMorrises.Contains(neighbours.Item1.First().Item1))
                        {
                            foreach (Tuple<GTMillGameElement, bool> element in neighbours.Item1)
                            {
                                this.diagonalMorrises.Add(element.Item1);
                            }

                            this.morrises++;
                        }

                        break;
                }
            }
        }

        /**
         * Checks 2 and 3 pieces configurations
         **/
        private void check23Pieces(int direction, Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>> neighbours)
        {
            //if there is 3 items then it will be a morris
            if (neighbours.Item1.Count == 2 && neighbours.Item2.Count == 0)
            {
                switch (direction)
                {
                    case 0:
                        if (!this.horizontal2Pieces.Contains(neighbours.Item1.First().Item1))
                        {
                            foreach (Tuple<GTMillGameElement, bool> element in neighbours.Item1)
                            {
                                this.horizontal2Pieces.Add(element.Item1);
                            }

                            this.twoPieces++;
                        }

                        break;
                    case 1:
                        if (!this.vertical2Pieces.Contains(neighbours.Item1.First().Item1))
                        {
                            foreach (Tuple<GTMillGameElement, bool> element in neighbours.Item1)
                            {
                                this.vertical2Pieces.Add(element.Item1);
                            }

                            this.twoPieces++;
                        }

                        break;
                    case 2:
                        if (!this.diagonal2Pieces.Contains(neighbours.Item1.First().Item1))
                        {
                            foreach (Tuple<GTMillGameElement, bool> element in neighbours.Item1)
                            {
                                this.diagonal2Pieces.Add(element.Item1);
                            }

                            this.twoPieces++;
                        }

                        break;
                }
            }
        }

        /**
         * Returns true if blocked in one direction
         **/
        private bool isBlocked(int direction, GTMillPosition position, Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>> neighbours)
        {
            List<Tuple<GTMillGameElement, bool>> opponentNeighbours 
                = (List<Tuple<GTMillGameElement, bool>>)neighbours.Item2.Where(item => item.Item2);
            
            bool blocked = true;

            if (((position.x == 0 || position.x == 2) && direction == 0)
                || ((position.y == 0 || position.y == 2) && direction == 1)
                || ((position.z == 0 || position.z == 2) && direction == 2))
            {
                blocked = (opponentNeighbours.Count > 0);
            }
            else
            {
                blocked = (opponentNeighbours.Count > 1);
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
                    //if possible to jump this direction
                    if (this.isValidDirection(direction, element.Key)) 
                    {
                        Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>> neighbours
                               = this.getNeighbours(direction, element.Key);

                        //if the next player has element at the position
                        if (element.Value.owner == state.nextPlayer)
                        {
                            //checking morrises
                            this.checkMorris(direction, neighbours);
                            //checking 2&3 pieces configuration 
                            this.check23Pieces(direction, neighbours);
                        }
                        else
                        {
                            //checking blocking
                            blocked = blocked && this.isBlocked(direction, element.Key, neighbours);
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

