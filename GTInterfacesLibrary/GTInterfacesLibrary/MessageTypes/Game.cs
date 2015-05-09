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
        /// The name of the game (should be unique).
        /// </summary>
        [XmlAttribute]
        public String Name { get; set; }

        /// <summary>
        /// The description of the game.
        /// </summary>
        [XmlAttribute]
        public String Description { get; set; }

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
    }
}
