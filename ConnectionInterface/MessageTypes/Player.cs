using System;
using System.Xml.Serialization;

namespace ConnectionInterface.MessageTypes
{
    public class Player
    {
        [XmlAttribute]
        public String Name { get; set; }

        public override Boolean Equals(Object obj)
        {
            if (obj is Player)
                return Name.Equals((obj as Player).Name);
            
            return false;
        }

        public override Int32 GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
