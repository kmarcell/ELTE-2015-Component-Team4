using System;
using GTInterfacesLibrary.MessageTypes;

namespace Server.Interfaces
{
    public interface IServerManager
    {
        String ServerIp { get; }

        Int32 ServerPort { get; }

        Boolean Running { get; }

        void Start();

        void Stop();

        void MessagePlayer(String player, MessageCode messageCode, Object messageContent);
    }
}
