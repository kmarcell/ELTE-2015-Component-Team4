using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using GTInterfacesLibrary;

namespace CheckersGame.Logic
{
    public class GameSpace : GTGameSpaceInterface<Element, Position>
    {
        private Dictionary<Position, Element> elements = new Dictionary<Position, Element>();
        private List<GTPlayerInterface<Element, Position>> _players = new List<GTPlayerInterface<Element, Position>>()
        {
            new GTPlayer<Element, Position>().playerWithRealUser(1),
            new GTPlayer<Element, Position>().playerWithRealUser(0),
        };

        public bool hasElementAt(Position p)
        {
            if (elements.Keys.Contains(p))
                return true;
            else
                return false;
        }

        public Element elementAt(Position p)
        {
            return elements[p];
        }

        public void setElementAt(Position p, Element e)
        {
            elements[p] = e;
        }

        public void addPlayer(GTPlayerInterface<Element, Position> player)
        {
            _players.Add(player);
        }

        public bool IsAI = false;

        public void mutateStateWith(GTGameStepInterface<Element, Position> step)
        {
            Step s = new Step(step.element, step.from, step.to);
            if (s.IsCapture())
            {
                Position pos = StepSupervisor.CapturedElementPos(s);
                elements.Remove(pos);

                //if (StepSupervisor.CanCapture())
                //nextP = 1 - step.element.owner;
            }

            elements.Remove(step.from);
            elements.Add(step.to, step.element);

            if (StepSupervisor.IsStepToKingsRow(s))
                step.element.type = 1;

            StepSupervisor.RefreshState(this);
        }

        public GTGameSpaceInterface<Element, Position> stateWithStep(GTGameStepInterface<Element, Position> step)
        {
            GameSpace newState = new GameSpace();
            newState.elements = new Dictionary<Position, Element>(this.elements);
            newState.mutateStateWith(step);
            return newState;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<Position, Element>> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        public Dictionary<Position, Element> GetElements()
        {
            return elements;

        }

        private int nextP = 1;
        public int nextPlayer
        {
            get { return nextP; }
            set { nextP = value; }
        }

        public void changePlayer()
        {
            if (IsAI == true)
                IsAI = false;
            else
                IsAI = true;
        }

        public GTPlayerInterface<Element, Position> getNextPlayer()
        {
            return _players[nextP];
        }
    }
}
