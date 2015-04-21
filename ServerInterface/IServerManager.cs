using System;
using ConnectionInterface.MessageTypes;

namespace ServerInterface
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
