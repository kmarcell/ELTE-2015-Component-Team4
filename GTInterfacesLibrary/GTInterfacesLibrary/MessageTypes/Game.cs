using System;
using System.Xml.Serialization;
using GTInterfacesLibrary.GameEvents;

namespace GTInterfacesLibrary.MessageTypes
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
        public String FirstPlayer { get; set; }

        [XmlAttribute]
        public String SecondPlayer { get; set; }

        [XmlAttribute]
        public String Winner { get; set; }

        [XmlAttribute]
        public GamePhase Phase { get; set; }

        [XmlAttribute]
        public String PlayerTurn { get; set; }

        [XmlAttribute]
        public Byte[] GameState { get; set; }
    }
}
