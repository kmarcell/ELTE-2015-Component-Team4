﻿using System;
using System.Xml.Serialization;

namespace Server.Utilities
{
    public class GameType
    {
        [XmlAttribute]
        public Int32 Id { get; set; }

        [XmlAttribute]
        public String Name { get; set; }

        [XmlAttribute]
        public String Description { get; set; }
    }
}
