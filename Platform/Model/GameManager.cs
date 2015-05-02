using System;
using System.Collections.Generic;
using System.IO;
using ConnectionInterface;
using ConnectionInterface.GameEvents;
using ConnectionInterface.MessageTypes;
using PlatformInterface;
using PlatformInterface.EventsGameRelated;
using PlatformInterface.EventsServerRelated;

namespace Platform.Model
{
    public class GameManager : IGameManager, IPlatformGameManager
    {
        private readonly NetworkManager _MNetworkManager;
        public GameManager(NetworkManager networkManager)
        {
            ArtificialIntelligences = new List<IArtificialIntelligence>();
            _MIsOnlineGame = true;
            _MNetworkManager = networkManager;
            _MNetworkManager.GameStatusReceived += RecieveGameStateFromNetwork;
        }

        private Boolean _MIsOnlineGame;
        public static IGame CurrentGame { get; private set; }

        public static Game CurrentNetworkGame { get; protected set; }

        public List<IArtificialIntelligence> ArtificialIntelligences { get; private set; }
        
        public event EventHandler<EventArgs>  GameStartedEvent;
        public event EventHandler<GameEndedEventArgs> GameEndedEvent;

        public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEvent;


        public void RegisterGame(IGame game)
        {
            CurrentGame = game;
            game.RegisterGameManager(this);
            game.SendGameStateChangedEventArg += RecieveGameStateFromLogic;
        }


        public void RegisterArtificialIntelligence(IArtificialIntelligence artificialIntelligence)
        {
            ArtificialIntelligences.Add(artificialIntelligence);
        }

        public void RecieveGameStateFromNetwork(object sender, GameEventArgs eventArgs)
        {
            var isMyTurn = eventArgs.Game.PlayerTurn == _MNetworkManager.PlayerName;
            var isWon = eventArgs.Game.Winner != null && (eventArgs.Game.Winner == _MNetworkManager.PlayerName);

            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = eventArgs.Game.Phase, GameState = eventArgs.Game.GameState, IsMyTurn = isMyTurn, IsWon = isWon, IsOnline = _MIsOnlineGame });
        }

        public void RecieveGameStateFromLogic(object sender, GameStateChangedEventArgs eventArgs)
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

                return;
            }

            
            // if online game, send it to the server
            CurrentNetworkGame.GameState = eventArgs.GameState;
            CurrentNetworkGame.Phase = eventArgs.GamePhase;

            CurrentNetworkGame.PlayerTurn = (CurrentNetworkGame.FirstPlayer == _MNetworkManager.PlayerName && eventArgs.IsMyTurn) 
                                                ? CurrentNetworkGame.FirstPlayer 
                                                : CurrentNetworkGame.SecondPlayer;
                

            if (eventArgs.GamePhase == GamePhase.Ended)
                CurrentNetworkGame.Winner = _MNetworkManager.PlayerName;


            _MNetworkManager.SendGameState(CurrentNetworkGame);
        }


        
        public void StartLocalGame(IArtificialIntelligence artificialIntelligence)
        {
            _MIsOnlineGame = false;
            CurrentGame.RegisterArtificialIntelligence(artificialIntelligence);
            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = GamePhase.Started, GameState = null, IsMyTurn = true, IsWon = false, IsOnline = _MIsOnlineGame });
        }

        public void EndLocalGame()
        {
            _MIsOnlineGame = true;
            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = GamePhase.Ended, GameState = null, IsMyTurn = false, IsWon = false, IsOnline = _MIsOnlineGame });
        }

        public void SaveLocalGame(String fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("Filename is empty.");

            var data = CurrentGame.SaveGame();
            File.WriteAllBytes(fileName, data);
        }

        public void LoadLocalGame(String fileName)
        {
            var data = File.ReadAllBytes(fileName);
            CurrentGame.LoadGame(data);
            _MIsOnlineGame = false;
        }
    }
}
