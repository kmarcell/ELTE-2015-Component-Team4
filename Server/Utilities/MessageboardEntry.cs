using System;
using System.Xml.Serialization;

namespace Server.Utilities
{
    public class MessageboardEntry
    {
        [XmlAttribute]
        public DateTime MessageTime { get; set; }

        [XmlAttribute]
        public String Player { get; set; }

        [XmlElement]
        public String Content { get; set; }
    }
}
