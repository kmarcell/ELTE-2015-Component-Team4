using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using GTInterfacesLibrary.MessageTypes;
using GameEndedEventArgs = Platform.Events.EventsGameRelated.GameEndedEventArgs;
using GameEventArgs = Platform.Events.EventsServerRelated.GameEventArgs;
using IGameManager = Platform.Model.Interface.IGameManager;

namespace Platform.Model
{
    public class GameManager : IGameManager, GTPlatformManagerInterface
    {
        private readonly NetworkManager _MNetworkManager;
        public GameManager(NetworkManager networkManager)
        {
            ArtificialIntelligenceList = new List<IGTArtificialIntelligenceInterface>();
            GameGuiList = new List<GTGuiInterface>();
            GameLogicList = new List<IGTGameLogicInterface>();

            _MGameType = GameType.Online;
            _MNetworkManager = networkManager;
            _MNetworkManager.GameStatusReceived += RecieveGameStateFromNetwork;
        }

        private GameType _MGameType;
        public static IGTGameLogicInterface CurrentGame { get; private set; }

        public static GTGuiInterface CurrentGui { get; private set; }

        public static Game CurrentNetworkGame { get; protected set; }

        public List<IGTArtificialIntelligenceInterface> ArtificialIntelligenceList { get; private set; }

        public List<GTGuiInterface> GameGuiList { get; private set; }

        public List<IGTGameLogicInterface> GameLogicList { get; private set; }
        
        public event EventHandler<EventArgs>  GameStartedEvent;
        public event EventHandler<GameEndedEventArgs> GameEndedEvent;

        public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEvent;


        #region initialize components

        public void InitializeGameLogic(string gameLogicDirectory)
        {
            var logicFromDirectory = Directory.GetFiles(gameLogicDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var logicDll in logicFromDirectory)
            {
                var gameAssembly = Assembly.LoadFrom(logicDll);
                Type gameType;

                try
                {
                    gameType = gameAssembly.GetTypes().FirstOrDefault(x => x.GetInterfaces().Any(y => y.Name.Contains("IGTGameLogicInterface") && !x.IsInterface));
                }
                catch (ReflectionTypeLoadException)
                {
                    gameType = null;
                }

                if (gameType == null)
                {
                    continue;
                }
            
            
                var gameLogicObject = (IGTGameLogicInterface)Activator.CreateInstance(gameType);
                GameLogicList.Add(gameLogicObject);
            }

            _MGameType = GameType.Online;
            CurrentGame = GameLogicList.First();
            CurrentGame.RegisterGameManager(this);
            CurrentGame.RegisterGui(CurrentGui);
            CurrentGame.SendGameStateChangedEventArg += RecieveGameStateFromLogic;
        }

        public void InitializeArtificialIntelligence(string artificialIntelligenceDirectory)
        {
            var aiFromDirectory = Directory.GetFiles(artificialIntelligenceDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var aiDll in aiFromDirectory)
            {
                var aiAssembly = Assembly.LoadFrom(aiDll);
                List<Type> aiType;

                try
                {
                    aiType = aiAssembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y.Name.Contains("IGTArtificialIntelligenceInterface"))).ToList();
                }
                catch (ReflectionTypeLoadException)
                {
                    aiType = null;
                }

                if (aiType == null)
                {
                    continue;
                }

                foreach (var currentAiType in aiType)
                {
                    var aiObject = Activator.CreateInstance(currentAiType);
                    ArtificialIntelligenceList.Add((IGTArtificialIntelligenceInterface)aiObject);
                }
            }
        }

        public void InitializeGui(List<GTGuiInterface> gameGuiList)
        {
            GameGuiList = gameGuiList;
            CurrentGui = GameGuiList.First();
        }

        public void InitializeGui(string guiDirectory)
        {
            var guiFromDirectory = Directory.GetFiles(guiDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var guiDll in guiFromDirectory)
            {
                var aiAssembly = Assembly.LoadFrom(guiDll);
                List<Type> aiType;

                try
                {
                    aiType = aiAssembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y.Name.Contains("GTGuiInterface"))).ToList();
                }
                catch (ReflectionTypeLoadException)
                {
                    aiType = null;
                }

                if (aiType == null)
                {
                    continue;
                }

                foreach (var currentAiType in aiType)
                {
                    var aiObject = Activator.CreateInstance(currentAiType);
                    GameGuiList.Add((GTGuiInterface)aiObject);
                }
            }

            CurrentGui = GameGuiList.First();
        }
        #endregion

        public void SetCurrentGame(IGTGameLogicInterface game)
        {
            CurrentGame = game;
        }

        public void SetCurrentGui(GTGuiInterface gui)
        {
            CurrentGui = gui;
            CurrentGame.RegisterGui(gui);
        }

        public void RecieveGameStateFromNetwork(object sender, GameEventArgs eventArgs)
        {
            var isMyTurn = eventArgs.Game.PlayerTurn == _MNetworkManager.PlayerName;
            var isWon = eventArgs.Game.Winner != null && (eventArgs.Game.Winner == _MNetworkManager.PlayerName);


            if (eventArgs.Game.Phase == GamePhase.Ended)
            {
                GameEndedEvent(this, new GameEndedEventArgs { IsEnded = true, IsWin = CurrentNetworkGame.Winner == _MNetworkManager.PlayerName });
            }

            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = eventArgs.Game.Phase, GameState = eventArgs.Game.GameState, IsMyTurn = isMyTurn, IsWon = isWon, GameType = _MGameType });
        }

        public void RecieveGameStateFromLogic(object sender, GameStateChangedEventArgs eventArgs)
        {
            if (_MGameType != GameType.Online)
            {
                switch (eventArgs.GamePhase)
                {
                    case GamePhase.Started:
                    case GamePhase.Playing:
                        return;

                    case GamePhase.Ended:
                        GameEndedEvent(this, new GameEndedEventArgs { IsEnded = true, IsWin = eventArgs.IsWon });
                        _MGameType = GameType.Online;
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
            {
                CurrentNetworkGame.Winner = _MNetworkManager.PlayerName;
                GameEndedEvent(this, new GameEndedEventArgs { IsEnded = true, IsWin = true });
            }


            _MNetworkManager.SendGameState(CurrentNetworkGame);
        }



        public void StartLocalGame(IGTArtificialIntelligenceInterface artificialIntelligence)
        {
            _MGameType = GameType.Local;
            CurrentGame.RegisterArtificialIntelligence(artificialIntelligence);
            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = GamePhase.Started, GameState = null, IsMyTurn = true, IsWon = false, GameType = _MGameType });
        }

        public void StartAiAiGame()
        {
            _MGameType = GameType.Ai;
            var randomToSelectAi = new Random().Next(0, ArtificialIntelligenceList.Count);
            CurrentGame.RegisterArtificialIntelligence(ArtificialIntelligenceList[randomToSelectAi]);
            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = GamePhase.Started, GameState = null, IsMyTurn = true, IsWon = false, GameType = _MGameType });
        }

        public void EndLocalGame()
        {
            _MGameType = GameType.Online;
            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = GamePhase.Ended, GameState = null, IsMyTurn = false, IsWon = false, GameType = _MGameType });
        }

        public void EndAiAiGame()
        {
            _MGameType = GameType.Online;
            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = GamePhase.Ended, GameState = null, IsMyTurn = false, IsWon = false, GameType = _MGameType });
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
            _MGameType = GameType.Local;
        }
    }
}
