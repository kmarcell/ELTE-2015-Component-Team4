using System.Collections.Generic;
using ConnectionInterface.MessageTypes;

namespace PlatformInterface
{
    public interface IDataManager
    {
        List<Game> Games { get; }

        Game GetCurrentGame { get; }

        void RegisterGame(Game game);
    }
}
