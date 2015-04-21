using System;
using ConnectionInterface.MessageTypes;

namespace ServerInterface
{
    public interface IDataManager
    {
        Game[] OnlineGames { get; }

        String[] OnlinePlayers { get; }

        String LoginPlayer(String playerName);

        void LogoutPlayer(String player);

        void CreateGame(Game game);

        Boolean JoinGame(Int32 gameId, String player);

        Game GetGame(Int32 gameId);

        Game[] GetOpenGames(String player, int gameTypeHashCode);

        void ChangeGameState(String player, Game game, Byte[] state);

        void EndGame(Game game, String player, String winner = null);
    }
}
