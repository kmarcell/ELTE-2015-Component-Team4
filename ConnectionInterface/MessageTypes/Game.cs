using System;
using System.Xml.Serialization;
using ConnectionInterface.GameEvents;

namespace ConnectionInterface.MessageTypes
{
    public class Game
    {
        [XmlAttribute]
        public Int32 Id { get; set; }

        [XmlAttribute]
        public String Name { get; set; }

        [XmlAttribute]
        public String Description { get; set; }

        [XmlAttribute]
        public int HashCode { get; set; }

        [XmlAttribute]
        public String FirstPlayer { get; set; }

        [XmlAttribute]
        public String SecondPlayer { get; set; }

        [XmlAttribute]
        public String Winner { get; set; }

        [XmlAttribute]
        public GamePhase Phase { get; set; }
    }
}
