using System;
using GTInterfacesLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CheckersGame.Logic
{
    public struct GameStateHashCoefficients
    {
        public int ownElementsNumber;
        public int opponentElementsNumber;
        public int ownKingsNumber;
        public int opponentKingsNumber;
        public int goalMinDistance;
        public int opponentMinDistance;
        public int ownHitCount;
        public int opponentHitCount;

        public GameStateHashCoefficients(
            int _ownElementsNumber,
            int _opponentElementsNumber,
            int _ownKingsNumber,
            int _opponentKingsNumber,
            int _goalMinDistance,
            int _opponentMinDistance,
            int _ownHitCount,
            int _opponentHitCount
        ) {
            ownElementsNumber = _ownElementsNumber;
            opponentElementsNumber = _opponentElementsNumber;
            ownKingsNumber = _ownKingsNumber;
            opponentKingsNumber = _opponentKingsNumber;
            goalMinDistance = _goalMinDistance;
            opponentMinDistance = _opponentMinDistance;
            ownHitCount = _ownHitCount;
            opponentHitCount = _opponentHitCount;
        }
    }

    public class GameStateHash : GTGameStateHashInterface<Element, Position>
    {
        public static GameStateHashCoefficients phase1Coefficients
            = new GameStateHashCoefficients(11, 10, 12, 10, 1, 1, 1, 1);

        public static GameStateHashCoefficients phase2Coefficients
            = new GameStateHashCoefficients(11, 10, 12, 10, 1, 1, 1, 1);

        public static GameStateHashCoefficients phase3Coefficients
            = new GameStateHashCoefficients(11, 10, 12, 10, 1, 1, 1, 1);

        private GTGameSpaceInterface<Element, Position> state;
        private GTPlayerInterface<Element,Position> player;

        private List<KeyValuePair<Position, Element>> opponentElements;
        private List<KeyValuePair<Position, Element>> ownElements;

        private List<KeyValuePair<Position, Element>> opponentKings;
        private List<KeyValuePair<Position, Element>> ownKings;

        int opponentHitCount;
        int ownHitCount;

        /**
         * True if we can jump in the given direction from the given position
         **/
        private bool isValidDirection(int direction, Position position, Element element)
        {
            switch (element.type)
            {
                case 0:
                    if (position.y != 7) 
                    { 
                        switch (position.x)
                        {
                            case 0:
                                if (direction == 1) return true;
                                break;
                            case 7:
                                if (direction == 0) return true;
                                break;
                            default:
                                if (direction == 0 || direction == 1) return true;
                                break;
                        }
                    }
                    break;
                case 1:
                    if (position.y == 7)
                    {
                        switch (position.x)
                        {
                            case 0:
                                if (direction == 3) return true;
                                break;
                            case 7:
                                if (direction == 2) return true;
                                break;
                            default:
                                if (direction == 2 || direction == 3) return true;
                                break;
                        }
                    }
                    else if (position.y == 0)
                    {
                        switch (position.x)
                        {
                            case 0:
                                if (direction == 1) return true;
                                break;
                            case 7:
                                if (direction == 0) return true;
                                break;
                            default:
                                if (direction == 1 || direction == 0) return true;
                                break;
                        }
                    }
                    else
                    {
                        switch (position.x)
                        {
                            case 0:
                                if (direction == 1 || direction == 3) return true;
                                break;
                            case 7:
                                if (direction == 0 || direction == 2) return true;
                                break;
                            default:
                                return true;
                        }
                    }

                    break;
            }

            return false;
        }

        private Position getOppositePosition(Position position, Position axis)
        {
            Position opposite = new Position(0,0);

            return opposite;
        }

        private bool canBeHit(Position position, Element element, Tuple<List<Tuple<Element, Position, bool>>, List<Tuple<Element, Position, bool>>, List<Position>> neighbours)
        {
            foreach (Tuple<Element, Position, bool> opponent in neighbours.Item2) 
            {
                if (opponent.Item2.y > position.y)
                {
                    return opponent.Item1.type == 1 && neighbours.Item3.Contains(this.getOppositePosition(opponent.Item2, position));
                }
                else if (opponent.Item2.y < position.y)
                {
                    return neighbours.Item3.Contains(this.getOppositePosition(opponent.Item2, position));
                }
            }

            return false;
        }

        private Tuple<List<Tuple<Element, Position, bool>>, List<Tuple<Element, Position, bool>>, HashSet<Position>> getNeighbours(Position position, Element element)
        {
            List<Tuple<Element, Position, bool>> opponentNeighbours = new List<Tuple<Element, Position, bool>>(),
                                                 ownNeighbours = new List<Tuple<Element, Position, bool>>();
            HashSet<Position> freePositions = new HashSet<Position>();

            if (position.y != 7) {
                if (position.x != 0)
                {
                    Position pos1 = new Position(position.x - 1, position.y + 1);
                    if (this.state.hasElementAt(pos1))
                    {
                        Element elem = this.state.elementAt(pos1);

                        if (elem.owner == element.owner)
                        {
                            ownNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos1, true));
                        }
                        else
                        {
                            opponentNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos1, true));
                        }
                    }
                    else
                    {
                        freePositions.Add(pos1);
                    }

                    Position pos2 = new Position(position.x - 1, position.y - 1);
                    if (this.state.hasElementAt(pos2))
                    {
                        Element elem = this.state.elementAt(pos2);

                        if (elem.owner == element.owner)
                        {
                            ownNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos2, element.type == 1));
                        }
                        else
                        {
                            opponentNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos2, element.type == 1));
                        }
                    }
                    else
                    {
                        freePositions.Add(pos2);
                    }
                }

                if (position.x != 7)
                {
                    Position pos1 = new Position(position.x + 1, position.y + 1);
                    if (this.state.hasElementAt(pos1))
                    {
                        Element elem = this.state.elementAt(pos1);

                        if (elem.owner == element.owner)
                        {
                            ownNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos1, true));
                        }
                        else
                        {
                            opponentNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos1, true));
                        }
                    }
                    else
                    {
                        freePositions.Add(pos1);
                    }

                    Position pos2 = new Position(position.x + 1, position.y - 1);
                    if (this.state.hasElementAt(pos2))
                    {
                        Element elem = this.state.elementAt(pos2);

                        if (elem.owner == element.owner)
                        {
                            ownNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos2, element.type == 1));
                        }
                        else
                        {
                            opponentNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos2, element.type == 1));
                        }
                    }
                    else
                    {
                        freePositions.Add(pos2);
                    }
                }
            }

            if (position.y != 0)
            {
                if (position.x != 0)
                {
                    Position pos1 = new Position(position.x - 1, position.y - 1);
                    if (this.state.hasElementAt(pos1))
                    {
                        Element elem = this.state.elementAt(pos1);

                        if (elem.owner == element.owner)
                        {
                            ownNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos1, element.type == 1));
                        }
                        else
                        {
                            opponentNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos1, element.type == 1));
                        }
                    }
                    else
                    {
                        freePositions.Add(pos1);
                    }
                }

                if (position.x != 7)
                {
                    Position pos1 = new Position(position.x + 1, position.y - 1);
                    if (this.state.hasElementAt(pos1))
                    {
                        Element elem = this.state.elementAt(pos1);

                        if (elem.owner == element.owner)
                        {
                            ownNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos1, element.type == 1));
                        }
                        else
                        {
                            opponentNeighbours.Add(new Tuple<Element, Position, bool>(elem, pos1, element.type == 1));
                        }
                    }
                    else
                    {
                        freePositions.Add(pos1);
                    }
                }
            }

            Tuple<List<Tuple<Element, Position, bool>>, List<Tuple<Element, Position, bool>>, HashSet<Position>> neighbours
                = new Tuple<List<Tuple<Element, Position, bool>>, List<Tuple<Element, Position, bool>>, HashSet<Position>>(ownNeighbours, opponentNeighbours, freePositions);

            return neighbours;
        }

        private int minOpponentDistance()
        {
            int distance = Int32.MaxValue;

            foreach (KeyValuePair<Position, Element> ownElem in this.ownElements)
            {
                foreach (KeyValuePair<Position, Element> opponentElem in this.ownElements)
                {
                    int currentDistance 
                        = Math.Max(Math.Abs(ownElem.Key.x - opponentElem.Key.x), Math.Abs(ownElem.Key.y - opponentElem.Key.y));

                    if (distance > currentDistance)
                    {
                        distance = currentDistance;
                    }

                    if (distance == 1)
                    {
                        return distance;
                    }
                }
            }

            return distance;
        }

        private int minGoalDistance()
        {
            int distance = 7;

            foreach (KeyValuePair<Position, Element> ownElem in this.ownElements)
            {
                if (distance > 7 - ownElem.Key.y)
                {
                    distance = ownElem.Key.y;
                }

                if (distance == 0)
                {
                    return 0;
                }
            }

            return distance;
        }

        public int evaluateState(GTGameSpaceInterface<Element, Position> state, GTPlayerInterface<Element, Position> player)
        {
            this.state = state;
            this.player = player;

            this.opponentHitCount = 0;
            this.ownHitCount = 0;

            this.opponentElements = new List<KeyValuePair<Position, Element>>();
            this.ownElements = new List<KeyValuePair<Position, Element>>();

            foreach (KeyValuePair<Position, Element> element in state)
            {
                if (element.Value.owner == this.player.id)
                {
                    this.ownElements.Add(element);

                    if (element.Value.type == 1)
                    {
                        this.ownKings.Add(element);
                    }
                }
                else
                {
                    this.opponentElements.Add(element);

                    if (element.Value.type == 1)
                    {
                        this.opponentKings.Add(element);
                    }
                }
            }

            int factor = 0;

            if (this.ownKings.Count == 0)
            {
                factor += phase1Coefficients.ownElementsNumber * this.ownElements.Count;
                factor += phase1Coefficients.opponentElementsNumber * this.opponentElements.Count;
                factor += phase1Coefficients.ownKingsNumber * this.ownKings.Count;
                factor += phase1Coefficients.opponentKingsNumber * this.opponentKings.Count;
                factor += phase1Coefficients.goalMinDistance * this.minGoalDistance();
            }
            else if (this.ownKings.Count > 0 && this.ownElements.Count != this.ownKings.Count)
            {
                factor += phase2Coefficients.ownElementsNumber * this.ownElements.Count;
                factor += phase2Coefficients.opponentElementsNumber * this.opponentElements.Count;
                factor += phase2Coefficients.ownKingsNumber * this.ownKings.Count;
                factor += phase2Coefficients.opponentKingsNumber * this.opponentKings.Count;
                factor += phase1Coefficients.goalMinDistance * this.minGoalDistance();
            }
            else
            {
                factor += phase3Coefficients.ownElementsNumber * this.ownElements.Count;
                factor += phase3Coefficients.opponentElementsNumber * this.opponentElements.Count;
                factor += phase3Coefficients.ownKingsNumber * this.ownKings.Count;
                factor += phase3Coefficients.opponentKingsNumber * this.opponentKings.Count;
                factor += phase3Coefficients.opponentMinDistance * this.minOpponentDistance();
            }

            return factor;
        }
    }
}
