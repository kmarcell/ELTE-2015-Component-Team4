using System;
using System.Collections.Generic;
using System.Linq;
using ConnectionInterface.MessageTypes;

namespace Server
{
    public class DataManager
    {
        private static DataManager _mDataManagerInstance;

        public static DataManager DataManagerInstance
        {
            get
            {
                return _mDataManagerInstance ?? (_mDataManagerInstance = new DataManager());
            }
        }

        private readonly List<Game> _OnlineGames;
        private readonly List<Player> _OnlinePlayers;

        public Game[] OnlineGames
        {
            get { return _OnlineGames.ToArray(); }
        }

        public Player[] OnlinePlayers
        {
            get { return _OnlinePlayers.ToArray(); }
        }

        private DataManager()
        {
            _OnlinePlayers = new List<Player>();
            _OnlineGames = new List<Game>();
        }

        public Player LoginPlayer(String playerName)
        {
            if (_OnlinePlayers.Count(x => x.Name == playerName) != 0)
                return null;

            if (playerName != null)
            {
                var player = new Player
                {
                    Name = playerName,
                };

                _OnlinePlayers.Add(player);
                return player;
            }
                
            return null;
        }

        public void LogoutPlayer(Player player)
        {
            if (_OnlinePlayers.Contains(player))
            {
                _OnlinePlayers.Remove(player);
            }
        }

        public Int32 CreateGame(Game game)
        {
            var prevGame = _OnlineGames.FirstOrDefault(x => x.Phase != GamePhase.Completed && (game.FirstPlayer.Equals(x.FirstPlayer) || game.FirstPlayer.Equals(x.SecondPlayer)));
            if (prevGame != null)
                EndGame(prevGame, game.FirstPlayer);

            game.CreateTime = DateTime.Now;
            _OnlineGames.Add(game);
           return game.GameId;
        }

        public Boolean JoinGame(Int32 gameId, Player player)
        {
            var prevGame = _OnlineGames.FirstOrDefault(x => x.Phase != GamePhase.Completed && (player.Equals(x.FirstPlayer) || player.Equals(x.SecondPlayer)));
            if (prevGame != null)
                EndGame(prevGame, player);

            var game = _OnlineGames.FirstOrDefault(x => x.GameId == gameId);
            if (game != null)
                lock (game)
                {
                    if (game.Phase == GamePhase.Open)
                    {
                        game.SecondPlayer = player;
                        game.StartTime = DateTime.Now;
                        ServerManager.ServerManagerInstance.MessagePlayer(game.FirstPlayer, MessageCode.JoinGame, game);
                        return true;
                    }
                    
                    return false;
                }

            return false;
        }

        public Game GetGame(Int32 gameId)
        {
            return _OnlineGames.FirstOrDefault(x => x.GameId == gameId);
        }

        public Game[] GetOpenGames(Player player, Int32 gameTypeId)
        {
            return _OnlineGames.Where(x => x.Type.Id == gameTypeId && x.Phase == GamePhase.Open && !player.Equals(x.FirstPlayer) && !player.Equals(x.SecondPlayer)).ToArray();
        }

        public void ChangeGameState(Player player, Game game, Byte[] state)
        {
            lock (game)
            {
                if (game.Phase == GamePhase.Playing)
                {
                    ServerManager.ServerManagerInstance.MessagePlayer(
                        game.FirstPlayer.Equals(player) ? game.SecondPlayer : game.FirstPlayer, MessageCode.ChangeGameState, state);
                }
            }
        }

        public void EndGame(Game game, Player player, Player winner = null)
        {
            lock (game)
            {
                if (game.Phase == GamePhase.Playing)
                {
                    game.Winner = winner;
                    game.EndTime = DateTime.Now;
                    ServerManager.ServerManagerInstance.MessagePlayer(game.FirstPlayer, MessageCode.EndGame, game);
                    ServerManager.ServerManagerInstance.MessagePlayer(game.SecondPlayer, MessageCode.EndGame, game);
                }

                _OnlineGames.Remove(game);
            }
        }
    }
}
