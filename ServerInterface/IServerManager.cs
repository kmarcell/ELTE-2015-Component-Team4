using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        void MessagePlayer(Player player, MessageCode messageCode, Object messageContent);
    }
}
