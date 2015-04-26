using GTInterfacesLibrary;

namespace CheckersGame.Logic
{
    public class Element : GTGameSpaceElementInterface
    {
        public int id { get; set; }
        public int type { get; set; }
        public int owner { get; set; }

        public Element(int id, int type, int owner)
        {
            this.id = id;
            this.type = type;
            this.owner = owner;
        }
    }
}
