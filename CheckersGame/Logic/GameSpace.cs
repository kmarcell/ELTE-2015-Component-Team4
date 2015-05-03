using System.Collections.Generic;
using System.Collections;
using System.Linq;
using GTInterfacesLibrary;

namespace CheckersGame.Logic
{
    public class GameSpace : GTGameSpaceInterface<Element, Position>
    {
        private Dictionary<Position, Element> elements = new Dictionary<Position, Element>();

        public string MyColor;

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
    }
}
