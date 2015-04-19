
using System.Collections.Generic;
using ConnectionInterface.MessageTypes;

namespace Platform.Model
{
    public class DataManager
    {
        public List<Game> Games { get; protected set; }

        public Game GetCurrentGame { get { return Games[0]; } }

        public DataManager()
        {
            Games = new List<Game>();
        }

        public void RegisterGame(Game game)
        {
            Games.Add(game);
        }
    }
}
