using System.Collections.Generic;
using ConnectionInterface.MessageTypes;

namespace PlatformInterface
{
    public interface IDataManager
    {
        Game CurrentGame { get; }

        void RegisterGame(Game game);
    }
}
