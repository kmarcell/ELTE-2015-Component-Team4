using System;
using System.Collections.Generic;
using System.Linq;
using GTInterfacesLibrary.GameEvents;
using GTInterfacesLibrary.MessageTypes;
using Server.Interfaces;

namespace Server.Implementation
{
    public class DataManager : IDataManager
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
        private readonly List<String> _OnlinePlayers;


        public String[] OnlinePlayers
        {
            get { return _OnlinePlayers.ToArray(); }
        }

        private DataManager()
        {
            _OnlinePlayers = new List<String>();
            _OnlineGames = new List<Game>();
        }

        public String LoginPlayer(String playerName)
        {
            lock (OnlinePlayers)
            {
                if (_OnlinePlayers.Count(x => x == playerName) != 0)
                    return null;

                if (playerName != null)
                {
                
                    _OnlinePlayers.Add(playerName);
                    return playerName;
                }
                
                return null;
            }
        }

        public void LogoutPlayer(String player)
        {
            if (_OnlinePlayers.Contains(player))
            {
                _OnlinePlayers.Remove(player);
            }
        }

        public void CreateGame(Game game)
        {
            var prevGame = _OnlineGames.FirstOrDefault(x => x.Phase != GamePhase.Ended && (game.FirstPlayer.Equals(x.FirstPlayer) || game.FirstPlayer.Equals(x.SecondPlayer)));
            if (prevGame != null)
                EndGame(prevGame, game.FirstPlayer);

            _OnlineGames.Add(game);
        }

        public Boolean JoinGame(Int32 gameId, String player)
        {
            var prevGame = _OnlineGames.FirstOrDefault(x => x.Phase != GamePhase.Ended && (player.Equals(x.FirstPlayer) || player.Equals(x.SecondPlayer)));
            if (prevGame != null)
                EndGame(prevGame, player);

            var game = _OnlineGames.FirstOrDefault(x => x.Id == gameId);
            if (game != null)
                lock (game)
                {
                    if (game.Phase == GamePhase.Opened)
                    {
                        game.Phase = GamePhase.Playing;
                        game.SecondPlayer = player;
                        ServerManager.ServerManagerInstance.MessagePlayer(game.FirstPlayer, MessageCode.JoinAccepted, game);
                        return true;
                    }
                    
                    return false;
                }

            return false;
        }

        public Game GetGame(Int32 gameId)
        {
            return _OnlineGames.FirstOrDefault(x => x.Id == gameId);
        }

        public Game[] GetOpenGames(String player, int id)
        {
            return _OnlineGames.Where(x => x.Phase == GamePhase.Opened && x.Id == id && !player.Equals(x.FirstPlayer) && !player.Equals(x.SecondPlayer)).ToArray();
        }

        public void ChangeGameState(String player, Game game)
        {
            lock (game)
            {
                if (game.Phase == GamePhase.Playing)
                {
                    ServerManager.ServerManagerInstance.MessagePlayer(
                        game.FirstPlayer.Equals(player) ? game.SecondPlayer : game.FirstPlayer, MessageCode.ChangeGameState, game);
                }
            }
        }

        public void EndGame(Game game, String player, String winner = null)
        {
            lock (game)
            {
                if (game.Phase == GamePhase.Playing)
                {
                    if (string.IsNullOrEmpty(winner))
                    {
                        game.Phase = GamePhase.Playing;
                        ServerManager.ServerManagerInstance.MessagePlayer(game.FirstPlayer, MessageCode.EndGame, game);
                        ServerManager.ServerManagerInstance.MessagePlayer(game.SecondPlayer, MessageCode.EndGame, game);
                    }
                    else
                    {
                        game.Phase = GamePhase.Ended;
                        game.Winner = winner;
                        ServerManager.ServerManagerInstance.MessagePlayer(game.FirstPlayer, MessageCode.EndGame, game);
                        ServerManager.ServerManagerInstance.MessagePlayer(game.SecondPlayer, MessageCode.EndGame, game);
                    }
                }

                _OnlineGames.Remove(game);
            }
        }
    }
}
