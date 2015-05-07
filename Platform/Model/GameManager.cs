using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using Platform.Model.Interface;
using GameEndedEventArgs = Platform.Events.EventsGameRelated.GameEndedEventArgs;
using GameEventArgs = Platform.Events.EventsServerRelated.GameEventArgs;
using IGameManager = Platform.Model.Interface.IGameManager;

namespace Platform.Model
{
    /// <summary>
    /// The GameManager in Platform. Which implements the <see cref="IGameManager"/> interface to handle user inputs on its GUI.
    /// In addition it also implements <see cref="GTPlatformManagerInterface"/> for communicating with <see cref="IGTGameLogicInterface"/>.
    /// </summary>
    public class GameManager : IGameManager, GTPlatformManagerInterface
    {
        #region private fields
        /// <summary>
        /// The instance of INetworkManager to check wether it sends a GameState from the other client.
        /// </summary>
        private readonly INetworkManager _MNetworkManager;

        /// <summary>
        /// The type of the started game.
        /// </summary>
        private GameType _MGameType;
        #endregion

        /// <summary>
        /// Create an instance of <see cref="GameManager"/>.
        /// </summary>
        /// <param name="networkManager">The networkmanager of the Platfrom.</param>
        public GameManager(INetworkManager networkManager)
        {
            ArtificialIntelligenceList = new List<IGTArtificialIntelligenceInterface>();
            GameGuiList = new List<GTGuiInterface>();
            GameLogicList = new List<IGTGameLogicInterface>();

            _MGameType = GameType.Online;
            _MNetworkManager = networkManager;
            _MNetworkManager.GameStatusReceived += RecieveGameStateFromNetwork;
        }


        #region component fields
        /// <summary>
        /// The instance of the currently loaded (selected) GameLogic in the platfrom.
        /// </summary>
        public IGTGameLogicInterface CurrentGame { get; private set; }

        /// <summary>
        /// The instance of the currently loaded (selected) Gui in the platfrom.
        /// </summary>
        public GTGuiInterface CurrentGui { get; private set; }

        /// <summary>
        /// The instance of the currently loaded (selected) ArtificialItelligence in the platfrom.
        /// </summary>
        public List<IGTArtificialIntelligenceInterface> ArtificialIntelligenceList { get; private set; }

        /// <summary>
        /// The list of available Gui instances in the platform.
        /// </summary>
        public List<GTGuiInterface> GameGuiList { get; private set; }

        /// <summary>
        /// The list of available GameLogic instances in the platform.
        /// </summary>
        public List<IGTGameLogicInterface> GameLogicList { get; private set; }
        #endregion


        #region event handlers
        /// <summary>
        /// The event  will raise when the game is started.
        /// </summary>
        /// <remarks>
        /// Event is for inform user about the game in the platform gui.
        /// </remarks>
        public event EventHandler<EventArgs> GameStartedEvent;

        /// <summary>
        /// The event will raise when the game is ended.
        /// </summary>
        /// <remarks>
        /// Event is for inform user about the game in the platform gui.
        /// </remarks>
        public event EventHandler<GameEndedEventArgs> GameEndedEvent;

        /// <summary>
        /// The message which will send to the gamelogic if there is any change in game
        /// </summary>
        public event EventHandler<GameStateChangedEventArgs> SendGameStateChangedEvent;
        #endregion


        #region initialize components
        /// <summary>
        /// Load the <see cref="IGTGameLogicInterface"/> components from DLL from the given directory.
        /// Create instance of all Game, and register in <see cref="GameLogicList"/>.
        /// Select one, register <see cref="GTGuiInterface"/> for it and register <see cref="IGameManager"/> instance.
        /// </summary>
        /// <param name="gameLogicDirectory">The directory where the <see cref="IGTGameLogicInterface"/> components should be.</param>
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

        /// <summary>
        /// Load the <see cref="IGTArtificialIntelligenceInterface"/> components from DLL from the given directory.
        /// Create instance of all AI, and register in <see cref="ArtificialIntelligenceList"/>.
        /// </summary>
        /// <param name="artificialIntelligenceDirectory">The directory where the <see cref="IGTArtificialIntelligenceInterface"/> components should be.</param>
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

        /// <summary>
        /// Set <see cref="GameGuiList"/> with the given parameter (list of <see cref="GTGuiInterface"/>).
        /// </summary>
        /// <param name="gameGuiList">The list of <see cref="GTGuiInterface"/> components.</param>
        public void InitializeGui(List<GTGuiInterface> gameGuiList)
        {
            GameGuiList = gameGuiList;
            CurrentGui = GameGuiList.First();
        }

        /// <summary>
        /// Load the <see cref="GTGuiInterface"/> components from DLL from the given directory.
        /// Create instance of all GUI, and register in <see cref="GameGuiList"/>.
        /// </summary>
        /// <param name="guiDirectory">The directory where the <see cref="GTGuiInterface"/> components should be.</param>
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


        #region set component
        /// <summary>
        /// Set current GameLogic which is set after selecting in the Platform Gui <see cref="IGTGameLogicInterface"/>.
        /// </summary>
        /// <param name="game">The currently selected GameLogic.</param>
        public void SetCurrentGame(IGTGameLogicInterface game)
        {
            CurrentGame = game;
        }

        /// <summary>
        /// Set current Gui which is set after selecting in the Platform Gui <see cref="GTGuiInterface"/> and register in the GameLogic.
        /// </summary>
        /// <param name="gui">The currently selected <see cref="GTGuiInterface"/>.</param>
        public void SetCurrentGui(GTGuiInterface gui)
        {
            CurrentGui = gui;
            CurrentGame.RegisterGui(gui);
        }
        #endregion


        #region game state event handlers
        /// <summary>
        /// The subscriber for event handler which will performed if there is game state <see cref="INetworkManager"/>.
        /// </summary>
        /// <param name="sender">the sender object <see cref="INetworkManager"/>.</param>
        /// <param name="eventArgs">The event args <see cref="GameEventArgs"/>.</param>
        public void RecieveGameStateFromNetwork(object sender, GameEventArgs eventArgs)
        {
            var isMyTurn = eventArgs.Game.PlayerTurn == _MNetworkManager.PlayerName;
            var isWon = eventArgs.Game.Winner != null && (eventArgs.Game.Winner == _MNetworkManager.PlayerName);


            if (eventArgs.Game.Phase == GamePhase.Ended)
            {
                GameEndedEvent(this, new GameEndedEventArgs { IsEnded = true, IsWin = _MNetworkManager.CurrentGame.Winner == _MNetworkManager.PlayerName });
            }

            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = eventArgs.Game.Phase, GameState = eventArgs.Game.GameState, IsMyTurn = isMyTurn, IsWon = isWon, GameType = _MGameType });
        }

        /// <summary>
        /// The subscriber for event of <see cref="IGTGameLogicInterface"/>.
        /// </summary>
        /// <param name="sender">the sender which is <see cref="IGTGameLogicInterface"/>.</param>
        /// <param name="eventArgs">the game state represented eventargs with all fields</param>
        public void RecieveGameStateFromLogic(object sender, GameStateChangedEventArgs eventArgs)
        {
            if (_MGameType != GameType.Online)
            {
                switch (eventArgs.GamePhase)
                {
                    case GamePhase.Started:
                        GameStartedEvent(this, EventArgs.Empty);
                        return;

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
            _MNetworkManager.CurrentGame.GameState = eventArgs.GameState;
            _MNetworkManager.CurrentGame.Phase = eventArgs.GamePhase;

            _MNetworkManager.CurrentGame.PlayerTurn = (_MNetworkManager.CurrentGame.FirstPlayer == _MNetworkManager.PlayerName && eventArgs.IsMyTurn)
                                                ? _MNetworkManager.CurrentGame.FirstPlayer
                                                : _MNetworkManager.CurrentGame.SecondPlayer;


            if (eventArgs.GamePhase == GamePhase.Ended)
            {
                _MNetworkManager.CurrentGame.Winner = _MNetworkManager.PlayerName;
                GameEndedEvent(this, new GameEndedEventArgs { IsEnded = true, IsWin = true });
            }


            _MNetworkManager.SendGameState(_MNetworkManager.CurrentGame);
        }
        #endregion


        #region menu events
        /// <summary>
        /// Start local game against the selected AI <see cref="IGTArtificialIntelligenceInterface"/>.
        /// Register AI in GameLogic <see cref="CurrentGame"/>, set GameType, SendGameStateChangedEvent to GameLogic and inform player in Platform GUI with GameStartedEvent.
        /// </summary>
        /// <param name="artificialIntelligence">The selected AI, one from the loaded ones.</param>
        public void StartLocalGame(IGTArtificialIntelligenceInterface artificialIntelligence)
        {
            _MGameType = GameType.Local;
            CurrentGame.RegisterArtificialIntelligence(artificialIntelligence);
            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = GamePhase.Started, GameState = null, IsMyTurn = true, IsWon = false, GameType = _MGameType });
            GameStartedEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// Start AI-AI <see cref="IGTArtificialIntelligenceInterface"/>.
        /// Register AI in GameLogic <see cref="CurrentGame"/>, set GameType, SendGameStateChangedEvent to GameLogic and inform player in Platform GUI with GameStartedEvent.
        /// </summary>
        public void StartAiAiGame()
        {
            _MGameType = GameType.Ai;
            var randomToSelectAi = new Random().Next(0, ArtificialIntelligenceList.Count);
            CurrentGame.RegisterArtificialIntelligence(ArtificialIntelligenceList[randomToSelectAi]);
            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = GamePhase.Started, GameState = null, IsMyTurn = true, IsWon = false, GameType = _MGameType });
            GameStartedEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// End local game.
        /// Set GameType, and SendGameStateChangedEvent to GameLogic and inform player in Platform GUI with GameEndedEvent.
        /// </summary>
        public void EndLocalGame()
        {
            _MGameType = GameType.Online;
            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = GamePhase.Ended, GameState = null, IsMyTurn = false, IsWon = false, GameType = _MGameType });
            GameEndedEvent(this, new GameEndedEventArgs{ IsEnded = false, IsWin = false });
        }

        /// <summary>
        /// Start AI-AI.
        /// Set GameType, and SendGameStateChangedEvent to GameLogic and inform player in Platform GUI with GameEndedEvent.
        /// </summary>
        public void EndAiAiGame()
        {
            _MGameType = GameType.Online;
            SendGameStateChangedEvent(this, new GameStateChangedEventArgs { GamePhase = GamePhase.Ended, GameState = null, IsMyTurn = false, IsWon = false, GameType = _MGameType });
            GameEndedEvent(this, new GameEndedEventArgs { IsEnded = false, IsWin = false });
        }

        /// <summary>
        /// Save local game if the game is started and the path with filename is correct.
        /// </summary>
        /// <param name="fileName">The filename of the saved data.</param>
        public void SaveLocalGame(String fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("Filename is empty.");

            var data = CurrentGame.SaveGame();
            File.WriteAllBytes(fileName, data);
        }

        /// <summary>
        /// Load local game if no game is started and the path with filename is correct.
        /// </summary>
        /// <param name="fileName">The filename of the load data.</param>
        public void LoadLocalGame(String fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("Filename is empty.");

            var data = File.ReadAllBytes(fileName);
            CurrentGame.LoadGame(data);
            _MGameType = GameType.Local;
        }
        #endregion
    }
}
