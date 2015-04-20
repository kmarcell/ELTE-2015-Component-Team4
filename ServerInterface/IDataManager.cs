using System;
using ConnectionInterface.MessageTypes;

namespace ServerInterface
{
    public interface IDataManager
    {
        Game[] OnlineGames { get; }

        Player[] OnlinePlayers { get; }

        Player LoginPlayer(String playerName);

        void LogoutPlayer(Player player);

        void CreateGame(Game game);

        Boolean JoinGame(Int32 gameId, Player player);

        Game GetGame(Int32 gameId);

        Game[] GetOpenGames(Player player, int gameTypeHashCode);

        void ChangeGameState(Player player, Game game, Byte[] state);

        void EndGame(Game game, Player player, Player winner = null);
    }
}
