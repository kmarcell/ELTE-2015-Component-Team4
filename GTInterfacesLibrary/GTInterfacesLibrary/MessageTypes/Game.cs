using System;
using System.Xml.Serialization;
using GTInterfacesLibrary.GameEvents;

namespace GTInterfacesLibrary.MessageTypes
{
    /// <summary>
    /// The Game class which is send on network as xml message.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The id of the game (should be unique).
        /// </summary>
        [XmlAttribute]
        public Int32 Id { get; set; }

        /// <summary>
        /// The id of the game type (should be unique).
        /// </summary>
        [XmlAttribute]
        public Int32 TypeId { get; set; }

        /// <summary>
        /// The name of the game type (should be unique).
        /// </summary>
        [XmlAttribute]
        public String TypeName { get; set; }

        /// <summary>
        /// The description of the gam typee.
        /// </summary>
        [XmlAttribute]
        public String TypeDescription { get; set; }

        /// <summary>
        /// The first player name of the game (creator).
        /// </summary>
        [XmlAttribute]
        public String FirstPlayer { get; set; }

        /// <summary>
        /// The second player name of the game (joiner).
        /// </summary>
        [XmlAttribute]
        public String SecondPlayer { get; set; }

        /// <summary>
        /// The name of the winner.
        /// </summary>
        [XmlAttribute]
        public String Winner { get; set; }

        /// <summary>
        /// The game phase <see cref="GamePhase"/>.
        /// </summary>
        [XmlAttribute]
        public GamePhase Phase { get; set; }

        /// <summary>
        /// The name of the player who should step as next.
        /// </summary>
        [XmlAttribute]
        public String PlayerTurn { get; set; }

        /// <summary>
        /// The current state of the game which can be interpreted by the GameLogic.
        /// </summary>
        [XmlAttribute]
        public Byte[] GameState { get; set; }

        public override string ToString()
        {
            return string.Format("TypeId: {0}, TypeName: {1}, TypeDescription: {2}, FirstPlayer: {3}, SecondPlayer: {4}, Phase: {5}", 
                TypeId, TypeName, TypeDescription, FirstPlayer ?? "null", SecondPlayer ?? "null", Phase);
        }
    }
}
