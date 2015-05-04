using System;
using System.IO;
using System.Xml.Serialization;
using GTInterfacesLibrary.GameEvents;

namespace GTInterfacesLibrary.MessageTypes
{
    public enum MessageCode {
        Login, Disconnect, ConnectAccepted, ConnectRejected, CreateGame, JoinGame, JoinAccepted, JoinRejected, EndGame, ChangeGameState, GetOpenGames
    }

    [XmlRoot]
    [XmlInclude(typeof(GamePhase)),
     XmlInclude(typeof(MessageCode)), 
     XmlInclude(typeof(Game)), 
     XmlInclude(typeof(Game[])),
     XmlInclude(typeof(Byte[]))]
    public class Message
    {
        [XmlAttribute]
        public MessageCode Code { get; set; }

        [XmlElement]
        public Object Content { get; set; }

        public static Byte[] Serialize(MessageCode code, Object param)
        {
            var message = new Message { Code = code, Content = param };

            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(Message));
                serializer.Serialize(stream, message);
                return stream.ToArray();
            }
        }

        public static Message Deserialize(Byte[] buffer, Int32 length)
        {
            using (var stream = new MemoryStream(buffer, 0, length))
            {
                var serializer = new XmlSerializer(typeof(Message));
                var message = serializer.Deserialize(stream) as Message;
                return message;
            }
        }
    }
}
