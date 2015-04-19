
using System.Collections.Generic;
using ConnectionInterface.MessageTypes;
using PlatformInterface;

namespace Platform.Model
{
    public class DataManager : IDataManager
    {
        public Game CurrentGame { get; protected set; }

        public void RegisterGame(Game game)
        {
            CurrentGame = game;
        }
    }
}
