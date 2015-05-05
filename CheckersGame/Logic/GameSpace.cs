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

        public void mutateStateWith(GTGameStepInterface<Element, Position> step)
        {
            elements.Remove(step.from);
            elements.Add(step.to, step.element);

            Step s = new Step(step.element, step.from, step.to);

            if (StepSupervisor.IsStepToKingsRow(s))
                step.element.type = 1;
            
            if (s.IsCapture())
            {
                Position pos = StepSupervisor.CapturedElementPos(s);
                elements.Remove(pos);
            }

            if (!StepSupervisor.CanCapture())
                nextP = 1 - step.element.owner;

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

        public bool CanCaptureFromState(GameSpace state)
        {
            throw new NotImplementedException();
        }

        private int nextP = 1;
        public int nextPlayer
        {
            get { return nextP; }
        }

        public int gamePhase
        {
            get
            {
                return 0;
            }
        }
    }
}
