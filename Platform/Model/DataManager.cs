using ConnectionInterface;

namespace Platform.Model
{
    public static class DataManager
    {
        public static IGame CurrentGame { get; private set; }

        public static void RegisterGame(IGame game)
        {
            // todo, generate gameId from dll
            CurrentGame = game;
        }
    }
}
