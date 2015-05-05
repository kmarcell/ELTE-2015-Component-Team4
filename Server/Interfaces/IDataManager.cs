using System;
using GTInterfacesLibrary.MessageTypes;

namespace Server.Interfaces
{
    public interface IDataManager
    {
        String[] OnlinePlayers { get; }

        String LoginPlayer(String playerName);

        void LogoutPlayer(String player);

        void CreateGame(Game game);

        Boolean JoinGame(Int32 gameId, String player);

        Game GetGame(Int32 gameId);

        Game[] GetOpenGames(String player, int id);

        void ChangeGameState(String player, Game game);

        void EndGame(Game game, String player, String winner = null);
    }
}
