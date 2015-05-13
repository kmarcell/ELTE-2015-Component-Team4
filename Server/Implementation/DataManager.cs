using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GTInterfacesLibrary.GameEvents;
using GTInterfacesLibrary.MessageTypes;
using Server.Interfaces;

namespace Server.Implementation
{
    /// <summary>
    /// The DataManager class represenst the list of methods for storing, maintaining game actions on server.
    /// </summary>
    public class DataManager : IDataManager
    {
        #region private fields
        /// <summary>
        /// The instance of datamanager for singleton mode.
        /// </summary>
        private static DataManager _mDataManagerInstance;

        /// <summary>
        /// The list of the online games.
        /// </summary>
        private readonly List<Game> _OnlineGames;

        /// <summary>
        /// The list of the online players.
        /// </summary>
        private readonly List<String> _OnlinePlayers;

        /// <summary>
        /// The name of the logfile.
        /// </summary>
        private const String LogfileName = "server.log";

        /// <summary>
        /// The prefix of the log message.
        /// </summary>
        private String LogMessagePrefix
        {
            get { return string.Format("{0} - ", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")); }

        }

        /// <summary>
        /// The instance of <see cref="DataManager"/>.
        /// </summary>
        public static DataManager DataManagerInstance
        {
            get
            {
                return _mDataManagerInstance ?? (_mDataManagerInstance = new DataManager());
            }
        }
        #endregion


        #region constructor
        /// <summary>
        /// Create new instance of DataManager.
        /// </summary>
        private DataManager()
        {
            _OnlinePlayers = new List<String>();
            _OnlineGames = new List<Game>();
        }
        #endregion


        #region game related functions
        /// <summary>
        /// List of online players currently on the server.
        /// </summary>
        public String[] OnlinePlayers
        {
            get { return _OnlinePlayers.ToArray(); }
        }

        /// <summary>
        /// The method is performed when a client is logged in and decide if there is a user already logged in with the same name.
        /// </summary>
        /// <param name="playerName">The player name to login.</param>
        /// <returns>True if the user is logged in successfully, otherwise false.</returns>
        public bool LoginPlayer(String playerName)
        {
            lock (OnlinePlayers)
            {
                if (_OnlinePlayers.Count(x => x == playerName) != 0)
                {
                    WriteLog("Login player rejected due to name is already registered: {0}", playerName);
                    return false;
                }

                if (playerName != null)
                {
                    _OnlinePlayers.Add(playerName);
                    WriteLog("Login player accepted: {0}, added to OnlinePlayers.", playerName);
                    return true;
                }

                WriteLog("Login player rejected due to name is null.");
                return false;
            }
        }

        /// <summary>
        /// The method is performed when a client is logged out.
        /// </summary>
        /// <param name="playerName">The player name to log out.</param>
        public void LogoutPlayer(String playerName)
        {
            if (_OnlinePlayers.Contains(playerName))
            {
                _OnlinePlayers.Remove(playerName);
                WriteLog("Logout player accepted: {0}, deleted from OnlinePlayers.", playerName);

                var openedGameByUser = _OnlineGames.FirstOrDefault(game => game.FirstPlayer == playerName && game.Phase == GamePhase.Opened);
                if (openedGameByUser != null)
                {
                    _OnlineGames.Remove(openedGameByUser);
                    WriteLog("Opened game by player: {0}, deleted from OnlineGames.", playerName);
                }
            }
        }

        /// <summary>
        /// Create the given.
        /// </summary>
        /// <param name="game">The game to create.</param>
        public void CreateGame(Game game)
        {
            var prevGame = _OnlineGames.FirstOrDefault(x => x.Phase != GamePhase.Ended && (game.FirstPlayer.Equals(x.FirstPlayer) || game.FirstPlayer.Equals(x.SecondPlayer)));
            if (prevGame != null)
                EndGame(prevGame, game.FirstPlayer);

            lock (_OnlineGames)
            {
                game.Id = _OnlineGames.Count;
                _OnlineGames.Add(game);
            }

            WriteLog("Game created: {0}", game.ToString());
        }

        /// <summary>
        /// Join to the given game id.
        /// </summary>
        /// <param name="gameId">The id of the game, which the client would like to join.</param>
        /// <param name="playerName">The name of the player to join to the game.</param>
        /// <returns>True if the join was successful, otherwise false.</returns>
        public Boolean JoinGame(Int32 gameId, String playerName)
        {
            var prevGame = _OnlineGames.FirstOrDefault(x => x.Phase != GamePhase.Ended && (playerName.Equals(x.FirstPlayer) || playerName.Equals(x.SecondPlayer)));
            if (prevGame != null)
                EndGame(prevGame, playerName);

            var game = _OnlineGames.FirstOrDefault(x => x.Id == gameId);
            if (game != null)
                lock (game)
                {
                    if (game.Phase == GamePhase.Opened)
                    {
                        game.Phase = GamePhase.Started;
                        game.SecondPlayer = playerName;
                        game.PlayerTurn = game.FirstPlayer;
                        ServerManager.ServerManagerInstance.MessagePlayer(game.FirstPlayer, MessageCode.JoinAccepted, game);
                        WriteLog("Player: {0} joined to the game: {1}", playerName, game.ToString());
                        return true;
                    }

                    return false;
                }

            return false;
        }

        /// <summary>
        /// Get game from by TypeId.
        /// </summary>
        /// <param name="gameId">The id of the game.</param>
        /// <returns>If exists then the game which has the given TypeId, otherwise null.</returns>
        public Game GetGame(Int32 gameId)
        {
            var game = _OnlineGames.FirstOrDefault(x => x.Id == gameId);
            WriteLog("Online game by TypeId is: {0}", game == null ? "null" : game.ToString());
            return game;
        }

        /// <summary>
        /// Get the open games.
        /// </summary>
        /// <param name="player">The name of the player.</param>
        /// <param name="id">The id of the game.</param>
        /// <returns>The list of the opened games, which has the same id.</returns>
        public Game[] GetOpenGames(String player, Int32 id)
        {
            var onlineGames = _OnlineGames.Where(x => x.Phase == GamePhase.Opened && x.TypeId == id && !player.Equals(x.FirstPlayer) && !player.Equals(x.SecondPlayer)).ToArray();
            WriteLog("Online games: {0}", onlineGames.Select(x => x.ToString()).Aggregate((x, y) => string.Format("{0}, {1}", x, y)));
            return onlineGames;
        }
        
        /// <summary>
        /// Change game state on the other user if the user made a change.
        /// </summary>
        /// <param name="player">The player who made a change.</param>
        /// <param name="game">The game which contains all the relevant data of the change.</param>
        public void ChangeGameState(String player, Game game)
        {
            lock (game)
            {
                if(game.Phase == GamePhase.Started)
                    game.Phase = GamePhase.Playing;

                if (game.Phase == GamePhase.Playing)
                {
                    WriteLog("Game state changed: {0}", game.ToString());
                    ServerManager.ServerManagerInstance.MessagePlayer(game.FirstPlayer.Equals(player) ? game.SecondPlayer : game.FirstPlayer, MessageCode.ChangeGameState, game);
                }
            }
        }

        /// <summary>
        /// End the game.
        /// </summary>
        /// <param name="game">The game which should be ended.</param>
        /// <param name="player">The player who ends the game.</param>
        /// <param name="winner">The winner of the game, it can be null, if the game is cancelled.</param>
        public void EndGame(Game game, String player, String winner = null)
        {
            lock (game)
            {
                if (game.Phase == GamePhase.Playing)
                {
                    if (string.IsNullOrEmpty(winner))
                    {
                        game.Phase = GamePhase.Playing;
                        WriteLog("Game cancelled: {0}", game.ToString());
                        ServerManager.ServerManagerInstance.MessagePlayer(game.FirstPlayer, MessageCode.EndGame, game);
                        ServerManager.ServerManagerInstance.MessagePlayer(game.SecondPlayer, MessageCode.EndGame, game);
                    }
                    else
                    {
                        game.Phase = GamePhase.Ended;
                        game.Winner = winner;
                        WriteLog("Game finished : {0}", game.ToString());
                        ServerManager.ServerManagerInstance.MessagePlayer(game.FirstPlayer, MessageCode.EndGame, game);
                        ServerManager.ServerManagerInstance.MessagePlayer(game.SecondPlayer, MessageCode.EndGame, game);
                    }
                }
                else
                {
                    ServerManager.ServerManagerInstance.MessagePlayer(game.FirstPlayer, MessageCode.EndGame, game);
                }

                _OnlineGames.Remove(game);
                WriteLog("Game deleted from OnlineGames: {0}", game.ToString());
            }
        }
        #endregion


        #region logging
        /// <summary>
        /// Write the log into the logfile with the given message and parameters.
        /// </summary>
        /// <param name="message">the log message</param>
        /// <param name="messageParams">the paramters of the logmessage</param>
        private void WriteLog(string message, params object[] messageParams)
        {
            // create log
            if (!File.Exists(LogfileName))
            {
                File.Create(LogfileName);
            }

            using (var fileStream = new FileStream(LogfileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine("{0} {1}", LogMessagePrefix, string.Format(message, messageParams));
            }
        }
        #endregion
    }
}
