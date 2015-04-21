using System;
using System.IO;
using ConnectionInterface;
using ConnectionInterface.GameEvents;
using PlatformInterface;
using PlatformInterface.EventsGameRelated;

namespace Platform.Model
{
    public class GameManager : IGameManager, IPlatformGameManager
    {
        public GameManager()
        {
            _MIsOnlineGame = true;
        }

        private Boolean _MIsOnlineGame;
        public static IGame CurrentGame { get; private set; }

        public event EventHandler<EventArgs>  GameStartedEvent;
        public event EventHandler<GameEndedEventArgs> GameEndedEvent;

        public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEventArg;
        public event EventHandler<GameStateChangedEventArgs> SendNetworkGameStateChangedEventArg;


        public void RegisterGame(IGame game)
        {
            CurrentGame = game;
            game.RegisterGameManager(this);
            game.SendGameStateChangedEventArg += RecieveGameState;
        }

        public void RecieveNetworkGameState(object sender, GameStateChangedEventArgs eventArgs)
        {
            switch (eventArgs.GamePhase)
            {
                case GamePhase.Started:
                    SendGameState(new GameStateChangedEventArgs { GamePhase = GamePhase.Started, GameState = eventArgs.GameState, IsMyTurn = eventArgs.IsMyTurn, IsWon = eventArgs.IsWon, IsOnline = _MIsOnlineGame });
                    break;

                case GamePhase.Playing:
                    SendGameState(new GameStateChangedEventArgs { GamePhase = GamePhase.Playing, GameState = eventArgs.GameState, IsMyTurn = eventArgs.IsMyTurn, IsWon = eventArgs.IsWon, IsOnline = _MIsOnlineGame });
                    break;

                case GamePhase.Ended:
                    SendGameState(new GameStateChangedEventArgs { GamePhase = GamePhase.Ended, GameState = eventArgs.GameState, IsMyTurn = eventArgs.IsMyTurn, IsWon = eventArgs.IsWon, IsOnline = _MIsOnlineGame });
                    break;
            }
        }

        public void SendNetworkGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs)
        {
            SendNetworkGameStateChangedEventArg(this, currentGameStateChangedEventArgs);
        }

        
        public void SendGameState(GameStateChangedEventArgs currentGameStateChangedEventArgs)
        {
            SendGameStateChangedEventArg(this, currentGameStateChangedEventArgs);
        }

        public void RecieveGameState(object sender, GameStateChangedEventArgs eventArgs)
        {
            if (!_MIsOnlineGame)
            {
                switch (eventArgs.GamePhase)
                {
                    case GamePhase.Started:
                    case GamePhase.Playing:
                        return;

                    case GamePhase.Ended:
                        GameEndedEvent(this, new GameEndedEventArgs { IsEnded = true, IsWin = eventArgs.IsWon });
                        _MIsOnlineGame = true;
                        return;
                }
            }

            // TODO check
            // if online game, send it to the server
            // if local game and game is not ended, or started leave as it is
            switch (eventArgs.GamePhase)
            {
                case GamePhase.Started:
                    SendNetworkGameState(new GameStateChangedEventArgs { GamePhase = GamePhase.Started, GameState = eventArgs.GameState, IsMyTurn = eventArgs.IsMyTurn, IsWon = eventArgs.IsWon });
                    break;

                case GamePhase.Playing:
                    SendNetworkGameState(new GameStateChangedEventArgs { GamePhase = GamePhase.Playing, GameState = eventArgs.GameState, IsMyTurn = eventArgs.IsMyTurn, IsWon = eventArgs.IsWon });
                    break;

                case GamePhase.Ended:
                    SendNetworkGameState(new GameStateChangedEventArgs { GamePhase = GamePhase.Ended, GameState = eventArgs.GameState, IsMyTurn = eventArgs.IsMyTurn, IsWon = eventArgs.IsWon });
                    break;
            }
        }
        
        public void StartGame()
        {
            _MIsOnlineGame = false;
            SendGameState(new GameStateChangedEventArgs { GamePhase = GamePhase.Started, GameState = null, IsMyTurn = true, IsWon = false, IsOnline = _MIsOnlineGame });
        }

        public void EndGame()
        {
            _MIsOnlineGame = true;
            SendGameState(new GameStateChangedEventArgs { GamePhase = GamePhase.Ended, GameState = null, IsMyTurn = false, IsWon = false, IsOnline = _MIsOnlineGame });
        }

        public void SaveGame(String fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("Filename is empty.");

            var data = CurrentGame.SaveGame();
            File.WriteAllBytes(fileName, data);
        }

        public void LoadGame(String fileName)
        {
            var data = File.ReadAllBytes(fileName);
            CurrentGame.LoadGame(data);
            _MIsOnlineGame = false;
        }
    }
}
