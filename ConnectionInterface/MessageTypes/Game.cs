using System;
using System.Xml.Serialization;

namespace ConnectionInterface.MessageTypes
{
    public enum GamePhase { Open, Playing, Completed }

    [XmlInclude(typeof(Player))]
    public class Game
    {
        [XmlAttribute]
        public Int32 GameId { get; set; }

        [XmlElement]
        public GameType Type { get; set; }

        [XmlAttribute]
        public DateTime CreateTime { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [XmlAttribute]
        public String GameSize { get; set; }

        [XmlElement]
        public Player FirstPlayer { get; set; }

        [XmlElement]
        public Player SecondPlayer { get; set; }

        [XmlElement]
        public Player Winner { get; set; }

        public GamePhase Phase 
        {
            get {
                if (EndTime != null)
                    return GamePhase.Completed;
                if (StartTime != null)
                    return GamePhase.Playing;
                
                return GamePhase.Open;
            }
        }
    }
}
