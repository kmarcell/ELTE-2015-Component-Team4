using System;
using GTInterfacesLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace GTMillGameLogic
{
    public struct GTMillGameStateHashCoefficients
    {
        public int morrisesNumber;
        public int blockedOpponents;
        public int ownElementsNumber;
        public int opponentElementsNumber;
        public int twoPiecesConfiguration;
        public int threePiecesConfiguration;

        public GTMillGameStateHashCoefficients(
            int _morrisesNumber,
            int _blockedOpponents,
            int _ownElementsNumber,
            int _opponentElementsNumber,
            int _twoPiecesConfiguration,
            int _threePiecesConfiguration
        )
        {
            morrisesNumber = _morrisesNumber;
            blockedOpponents = _blockedOpponents;
            ownElementsNumber = _ownElementsNumber;
            opponentElementsNumber = _opponentElementsNumber;
            twoPiecesConfiguration = _twoPiecesConfiguration;
            threePiecesConfiguration = _threePiecesConfiguration;
        }
    }

    public class GTMillGameStateHash : GTGameStateHashInterface<GTMillGameElement, GTMillPosition>
    {
        public static GTMillGameStateHashCoefficients phase1Coefficients
            = new GTMillGameStateHashCoefficients(37, 4, 14, -14, 20, 25);

        public static GTMillGameStateHashCoefficients phase2Coefficients
            = new GTMillGameStateHashCoefficients(43, 11, 8, -8, 20, 25);

        public static GTMillGameStateHashCoefficients phase3Coefficients
            = new GTMillGameStateHashCoefficients(20, 0, 10, -10, 10, 15);

        private GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state;
        private GTPlayerInterface<GTMillGameElement, GTMillPosition> player;

        public List<KeyValuePair<GTMillPosition, GTMillGameElement>> opponentElements;
        public List<KeyValuePair<GTMillPosition, GTMillGameElement>> ownElements;

        private HashSet<GTMillGameElement> horizontalMorrises;
        private HashSet<GTMillGameElement> verticalMorrises;
        private HashSet<GTMillGameElement> diagonalMorrises;

        private HashSet<GTMillGameElement> horizontal2Pieces;
        private HashSet<GTMillGameElement> vertical2Pieces;
        private HashSet<GTMillGameElement> diagonal2Pieces;

        private HashSet<GTMillGameElement> horizontal3Pieces;
        private HashSet<GTMillGameElement> vertical3Pieces;
        private HashSet<GTMillGameElement> diagonal3Pieces;

        public int morrises;
        public int twoPiecesConfiguration;
        public int threePiecesConfiguration;
        public int blockedOpponents;
        public bool winningConfiguration;
        public bool losingConfiguration;

        /**
         * True if we can jump in the given direction from the given position
         **/
        private bool isValidDirection(int direction, GTMillPosition position)
        {
            return !(direction == 0 && position.y == 1) //you cant jump horizontal
                    && !(direction == 1 && position.x == 1) //you cant jump vertical
                    && !(direction == 2 && (
                    (position.x == 0 && position.y == 0)
                    || (position.x == 2 && position.y == 2)
                    || (position.x == 0 && position.y == 2)
                    || (position.x == 2 && position.y == 0)) //we can only jump diagonal from there
            );
        }

        /**
         * Returns the neighbour elements of the given position. 
         * The opponent and own elements will be seperated.
         **/
        private Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>, List<GTMillPosition>> getNeighbours(int direction, GTMillPosition position)
        {
            List<Tuple<GTMillGameElement, bool>> opponentNeighbours = new List<Tuple<GTMillGameElement, bool>>(),
                                                 ownNeighbours = new List<Tuple<GTMillGameElement, bool>>();
            List<GTMillPosition> freePositions = new List<GTMillPosition>();

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

                                if (element.owner == this.player.id)
                                {
                                    ownNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                                else
                                {
                                    opponentNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                            }
                            else
                            {
                                freePositions.Add(pos);
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

                                if (element.owner == this.player.id)
                                {
                                    ownNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                                else
                                {
                                    opponentNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                            }
                            else
                            {
                                freePositions.Add(pos);
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

                                if (element.owner == this.player.id)
                                {
                                    ownNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                                else
                                {
                                    opponentNeighbours.Add(new Tuple<GTMillGameElement, bool>(element, direct));
                                }
                            }
                            else
                            {
                                freePositions.Add(pos);
                            }
                        }

                        break;
                }
            }

            Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>, List<GTMillPosition>> neighbours
                = new Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>, List<GTMillPosition>>(ownNeighbours, opponentNeighbours, freePositions);

            return neighbours;
        }

        /**
         * Checks morrises in one direction
         **/
        private void checkMorris(int direction, Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>, List<GTMillPosition>> neighbours)
        {
            //there is a morris
            if (neighbours.Item1.Count == 3)
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
        private void check23Pieces(int direction, Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>, List<GTMillPosition>> neighbours)
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

                            this.twoPiecesConfiguration++;
                        }

                        if (!this.horizontal3Pieces.Contains(neighbours.Item1.First().Item1))
                        {
                            GTMillPosition freePos = neighbours.Item3.First();

                            for (int i = 0; i < 3; ++i)
                            {
                                if (i != direction)
                                {
                                    Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>, List<GTMillPosition>> ortogonalNeighbours
                                        = this.getNeighbours(i, freePos);
                                    List<Tuple<GTMillGameElement, bool>> ownOrtogonalNeighbours
                                        = new List<Tuple<GTMillGameElement, bool>>(ortogonalNeighbours.Item1.Where(item => item.Item2));

                                    if (ownOrtogonalNeighbours.Count > 0)
                                    {
                                        foreach (Tuple<GTMillGameElement, bool> ownElement in ownOrtogonalNeighbours)
                                        {
                                            this.horizontal3Pieces.Add(ownElement.Item1);
                                        }

                                        foreach (Tuple<GTMillGameElement, bool> element in neighbours.Item1)
                                        {
                                            this.horizontal3Pieces.Add(element.Item1);
                                        }

                                        this.threePiecesConfiguration++;
                                    }
                                }
                            }
                        }

                        break;
                    case 1:
                        if (!this.vertical2Pieces.Contains(neighbours.Item1.First().Item1))
                        {
                            foreach (Tuple<GTMillGameElement, bool> element in neighbours.Item1)
                            {
                                this.vertical2Pieces.Add(element.Item1);
                            }

                            this.twoPiecesConfiguration++;
                        }

                        if (!this.vertical3Pieces.Contains(neighbours.Item1.First().Item1))
                        {
                            GTMillPosition freePos = neighbours.Item3.First();

                            for (int i = 0; i < 3; ++i)
                            {
                                if (i != direction)
                                {
                                    Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>, List<GTMillPosition>> ortogonalNeighbours
                                        = this.getNeighbours(i, freePos);
                                    List<Tuple<GTMillGameElement, bool>> ownOrtogonalNeighbours
                                        = new List<Tuple<GTMillGameElement, bool>>(ortogonalNeighbours.Item1.Where(item => item.Item2));

                                    if (ownOrtogonalNeighbours.Count > 0)
                                    {
                                        foreach (Tuple<GTMillGameElement, bool> ownElement in ownOrtogonalNeighbours)
                                        {
                                            this.vertical3Pieces.Add(ownElement.Item1);
                                        }

                                        foreach (Tuple<GTMillGameElement, bool> element in neighbours.Item1)
                                        {
                                            this.vertical3Pieces.Add(element.Item1);
                                        }

                                        this.threePiecesConfiguration++;
                                    }
                                }
                            }
                        }

                        break;
                    case 2:
                        if (!this.diagonal2Pieces.Contains(neighbours.Item1.First().Item1))
                        {
                            foreach (Tuple<GTMillGameElement, bool> element in neighbours.Item1)
                            {
                                this.diagonal2Pieces.Add(element.Item1);
                            }

                            this.twoPiecesConfiguration++;
                        }

                        if (!this.diagonal3Pieces.Contains(neighbours.Item1.First().Item1))
                        {
                            GTMillPosition freePos = neighbours.Item3.First();

                            for (int i = 0; i < 3; ++i)
                            {
                                if (i != direction)
                                {
                                    Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>, List<GTMillPosition>> ortogonalNeighbours
                                        = this.getNeighbours(i, freePos);
                                    List<Tuple<GTMillGameElement, bool>> ownOrtogonalNeighbours
                                        = new List<Tuple<GTMillGameElement, bool>>(ortogonalNeighbours.Item1.Where(item => item.Item2));

                                    if (ownOrtogonalNeighbours.Count > 0)
                                    {
                                        foreach (Tuple<GTMillGameElement, bool> ownElement in ownOrtogonalNeighbours)
                                        {
                                            this.diagonal3Pieces.Add(ownElement.Item1);
                                        }

                                        foreach (Tuple<GTMillGameElement, bool> element in neighbours.Item1)
                                        {
                                            this.diagonal3Pieces.Add(element.Item1);
                                        }

                                        this.threePiecesConfiguration++;
                                    }
                                }
                            }
                        }

                        break;
                }
            }
        }

        /**
         * Returns true if blocked in one direction
         **/
        private bool isBlocked(int direction, GTMillPosition position, Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>, List<GTMillPosition>> neighbours)
        {
            List<Tuple<GTMillGameElement, bool>> ownNeighbours
                = new List<Tuple<GTMillGameElement, bool>>(neighbours.Item1.Where(item => item.Item2));

            bool blocked = true;

            if (((position.x == 0 || position.x == 2) && direction == 0)
                || ((position.y == 0 || position.y == 2) && direction == 1)
                || ((position.z == 0 || position.z == 2) && direction == 2))
            {
                blocked = (ownNeighbours.Count > 0);
            }
            else
            {
                blocked = (ownNeighbours.Count > 1);
            }

            return blocked;
        }

        private void evaulate()
        {
            this.opponentElements = new List<KeyValuePair<GTMillPosition, GTMillGameElement>>();
            this.ownElements = new List<KeyValuePair<GTMillPosition, GTMillGameElement>>();

            foreach (KeyValuePair<GTMillPosition, GTMillGameElement> element in state)
            {
                if (element.Value.owner == this.player.id)
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
                        Tuple<List<Tuple<GTMillGameElement, bool>>, List<Tuple<GTMillGameElement, bool>>, List<GTMillPosition>> neighbours
                               = this.getNeighbours(direction, element.Key);

                        //if the next player has element at the position
                        if (element.Value.owner == this.player.id)
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

                if (blocked && element.Value.owner != this.player.id)
                {
                    this.blockedOpponents++;
                }
            }
        }

        public int evaluateState(GTGameSpaceInterface<GTMillGameElement, GTMillPosition> state, GTPlayerInterface<GTMillGameElement, GTMillPosition> player)
        {
            this.state = state;
            this.player = player;

            this.morrises = 0;
            this.twoPiecesConfiguration = 0;
            this.threePiecesConfiguration = 0;
            this.blockedOpponents = 0;

            this.horizontalMorrises = new HashSet<GTMillGameElement>();
            this.verticalMorrises = new HashSet<GTMillGameElement>();
            this.diagonalMorrises = new HashSet<GTMillGameElement>();

            this.horizontal2Pieces = new HashSet<GTMillGameElement>();
            this.vertical2Pieces = new HashSet<GTMillGameElement>();
            this.diagonal2Pieces = new HashSet<GTMillGameElement>();

            this.horizontal3Pieces = new HashSet<GTMillGameElement>();
            this.vertical3Pieces = new HashSet<GTMillGameElement>();
            this.diagonal3Pieces = new HashSet<GTMillGameElement>();

            this.opponentElements = new List<KeyValuePair<GTMillPosition, GTMillGameElement>>();
            this.ownElements = new List<KeyValuePair<GTMillPosition, GTMillGameElement>>();

            this.evaulate();

            this.winningConfiguration = (this.ownElements.Count >= 3 && this.opponentElements.Count < 3);
            this.losingConfiguration = (this.ownElements.Count < 3 && this.opponentElements.Count >= 3);

            if (this.winningConfiguration)
            {
                return Int32.MaxValue;
            }

            if (this.losingConfiguration)
            {
                return Int32.MinValue;
            }

            int factor = 0;
            if (player.figuresRemaining > 0)
            {
                // first phase
                factor += phase1Coefficients.morrisesNumber * this.morrises;
                factor += phase1Coefficients.blockedOpponents * this.blockedOpponents;
                factor += phase1Coefficients.opponentElementsNumber * this.opponentElements.Count;
                factor += phase1Coefficients.ownElementsNumber * this.ownElements.Count;
                factor += phase1Coefficients.twoPiecesConfiguration * this.twoPiecesConfiguration;
                factor += phase1Coefficients.threePiecesConfiguration * this.threePiecesConfiguration;
            }
            else if (player.figuresInitial - player.figuresLost > 3)
            {
                // second phase
                factor += phase2Coefficients.morrisesNumber * this.morrises;
                factor += phase2Coefficients.blockedOpponents * this.blockedOpponents;
                factor += phase2Coefficients.opponentElementsNumber * this.opponentElements.Count;
                factor += phase2Coefficients.ownElementsNumber * this.ownElements.Count;
                factor += phase2Coefficients.twoPiecesConfiguration * this.twoPiecesConfiguration;
                factor += phase2Coefficients.threePiecesConfiguration * this.threePiecesConfiguration;
            }
            else
            {
                // third phase
                factor += phase3Coefficients.morrisesNumber * this.morrises;
                factor += phase3Coefficients.twoPiecesConfiguration * this.twoPiecesConfiguration;
                factor += phase3Coefficients.threePiecesConfiguration * this.threePiecesConfiguration;
            }

            return factor;
        }
    }

}

