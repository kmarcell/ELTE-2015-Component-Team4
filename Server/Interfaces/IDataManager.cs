using System;
using GTInterfacesLibrary.MessageTypes;

namespace Server.Interfaces
{
    /// <summary>
    /// The DataManager interface represenst the list of methods for storing, maintaining game actions on server.
    /// </summary>
    public interface IDataManager
    {
        #region game related functions
        /// <summary>
        /// List of online players currently on the server.
        /// </summary>
        String[] OnlinePlayers { get; }

        /// <summary>
        /// The method is performed when a client is logged in and decide if there is a user already logged in with the same name.
        /// </summary>
        /// <param name="playerName">The player name to login.</param>
        /// <returns>True if the user is logged in successfully, otherwise false.</returns>
        bool LoginPlayer(String playerName);

        /// <summary>
        /// The method is performed when a client is logged out.
        /// </summary>
        /// <param name="playerName">The player name to log out.</param>
        void LogoutPlayer(String playerName);

        /// <summary>
        /// Create the given.
        /// </summary>
        /// <param name="game">The game to create.</param>
        void CreateGame(Game game);

        /// <summary>
        /// Join to the given game id.
        /// </summary>
        /// <param name="gameId">The id of the game, which the client would like to join.</param>
        /// <param name="playerName">The name of the player to join to the game.</param>
        /// <returns>True if the join was successful, otherwise false.</returns>
        Boolean JoinGame(Int32 gameId, String playerName);

        /// <summary>
        /// Get game from by TypeId.
        /// </summary>
        /// <param name="gameId">The id of the game.</param>
        /// <returns>If exists then the game which has the given TypeId, otherwise null.</returns>
        Game GetGame(Int32 gameId);

        /// <summary>
        /// Get the open games.
        /// </summary>
        /// <param name="player">The name of the player.</param>
        /// <param name="id">The id of the game.</param>
        /// <returns>The list of the opened games, which has the same id.</returns>
        Game[] GetOpenGames(String player, Int32 id);

        /// <summary>
        /// Change game state on the other user if the user made a change.
        /// </summary>
        /// <param name="player">The player who made a change.</param>
        /// <param name="game">The game which contains all the relevant data of the change.</param>
        void ChangeGameState(String player, Game game);

        /// <summary>
        /// End the game.
        /// </summary>
        /// <param name="game">The game which should be ended.</param>
        /// <param name="player">The player who ends the game.</param>
        /// <param name="winner">The winner of the game, it can be null, if the game is cancelled.</param>
        void EndGame(Game game, String player, String winner = null);
        #endregion
    }
}
