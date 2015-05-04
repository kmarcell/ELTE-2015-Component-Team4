using GTInterfacesLibrary;
using System;

namespace CheckersGame.Logic
{
    public class Step : GTGameStepInterface<Element, Position>
    {
        public Element element { get; set; }
        public Position from { get; set; }
        public Position to { get; set; }

        public GTGameStepInterface<Element, Position> Create(Element e, Position from, Position to)
        {
            return new Step(e, from, to);
        }

        public Step(Element element, Position from, Position to)
        {
            this.from = from;
            this.to = to;
            this.element = element;
        }

        public bool IsCapture()
        {
            if (Math.Abs(from.x - to.x) == 2)
                return true;
            else
                return false;
        }
    }
}
