using System;
using System.Collections.Generic;
using GTInterfacesLibrary;
using GameEndedEventArgs = Platform.Events.EventsGameRelated.GameEndedEventArgs;
using GameEventArgs = Platform.Events.EventsServerRelated.GameEventArgs;

namespace Platform.Model.Interface
{
    /// <summary>
    /// The interface of GameManager in Platform. 
    /// It should manage the game through user reaction f.e.: selecting game, gui, start game, end game, etc.
    /// </summary>
    public interface IGameManager
    {
        #region component fields
        /// <summary>
        /// The instance of the currently loaded (selected) GameLogic in the platfrom.
        /// </summary>
        IGTGameLogicInterface CurrentGame { get; }

        /// <summary>
        /// The instance of the currently loaded (selected) Gui in the platfrom.
        /// </summary>
        GTGuiInterface CurrentGui { get; }

        /// <summary>
        /// The instance of the currently loaded (selected) ArtificialItelligence in the platfrom.
        /// </summary>
        List<IGTArtificialIntelligenceInterface> ArtificialIntelligenceList { get; }

        /// <summary>
        /// The list of available Gui instances in the platform.
        /// </summary>
        List<GTGuiInterface> GameGuiList { get; }

        /// <summary>
        /// The list of available GameLogic instances in the platform.
        /// </summary>
        List<IGTGameLogicInterface> GameLogicList { get; }
        #endregion


        #region event handlers
        /// <summary>
        /// The event  will raise when the game is started.
        /// </summary>
        /// <remarks>
        /// Event is for inform user about the game in the platform gui.
        /// </remarks>
        event EventHandler<EventArgs> GameStartedEvent;

        /// <summary>
        /// The event will raise when the game is ended.
        /// </summary>
        /// <remarks>
        /// Event is for inform user about the game in the platform gui.
        /// </remarks>
        event EventHandler<GameEndedEventArgs> GameEndedEvent;
        #endregion


        #region initialize components
        /// <summary>
        /// Load the <see cref="IGTGameLogicInterface"/> components from DLL from the given directory.
        /// Create instance of all Game, and register in <see cref="GameLogicList"/>.
        /// Select one, register <see cref="GTGuiInterface"/> for it and register <see cref="IGameManager"/> instance.
        /// </summary>
        /// <param name="gameLogicDirectory">The directory where the <see cref="IGTGameLogicInterface"/> components should be.</param>
        void InitializeGameLogic(string gameLogicDirectory);

        /// <summary>
        /// Load the <see cref="IGTArtificialIntelligenceInterface"/> components from DLL from the given directory.
        /// Create instance of all AI, and register in <see cref="ArtificialIntelligenceList"/>.
        /// </summary>
        /// <param name="artificialIntelligenceDirectory">The directory where the <see cref="IGTArtificialIntelligenceInterface"/> components should be.</param>
        void InitializeArtificialIntelligence(string artificialIntelligenceDirectory);

        /// <summary>
        /// Set <see cref="GameGuiList"/> with the given parameter (list of <see cref="GTGuiInterface"/>).
        /// </summary>
        /// <param name="gameGuiList">The list of <see cref="GTGuiInterface"/> components.</param>
        void InitializeGui(List<GTGuiInterface> gameGuiList);

        /// <summary>
        /// Load the <see cref="GTGuiInterface"/> components from DLL from the given directory.
        /// Create instance of all GUI, and register in <see cref="GameGuiList"/>.
        /// </summary>
        /// <param name="guiDirectory">The directory where the <see cref="GTGuiInterface"/> components should be.</param>
        void InitializeGui(string guiDirectory);
        #endregion


        #region set component
        /// <summary>
        /// Set current GameLogic which is set after selecting in the Platform Gui <see cref="IGTGameLogicInterface"/>.
        /// </summary>
        /// <param name="game">The currently selected <see cref="IGTGameLogicInterface"/>.</param>
        void SetCurrentGame(IGTGameLogicInterface game);

        /// <summary>
        /// Set current Gui which is set after selecting in the Platform Gui <see cref="GTGuiInterface"/> and register in the GameLogic.
        /// </summary>
        /// <param name="gui">The currently selected <see cref="GTGuiInterface"/>.</param>
        void SetCurrentGui(GTGuiInterface gui);
        #endregion


        #region game state event handlers
        /// <summary>
        /// The subscriber for event handler which will performed if there is game state <see cref="INetworkManager"/>.
        /// </summary>
        /// <param name="sender">the sender object <see cref="INetworkManager"/>.</param>
        /// <param name="eventArgs">The event args <see cref="GameEventArgs"/>.</param>
        void RecieveGameStateFromNetwork(object sender, GameEventArgs eventArgs);
        #endregion


        #region menu events
        /// <summary>
        /// Start local game against the selected AI <see cref="IGTArtificialIntelligenceInterface"/>.
        /// Register AI in GameLogic <see cref="CurrentGame"/>, set GameType, SendGameStateChangedEvent to GameLogic and inform player in Platform GUI with GameStartedEvent.
        /// </summary>
        /// <param name="artificialIntelligence">The selected AI, one from the loaded ones.</param>
        void StartLocalGame(IGTArtificialIntelligenceInterface artificialIntelligence);

        /// <summary>
        /// Start AI-AI <see cref="IGTArtificialIntelligenceInterface"/>.
        /// Register AI in GameLogic <see cref="CurrentGame"/>, set GameType, SendGameStateChangedEvent to GameLogic and inform player in Platform GUI with GameStartedEvent.
        /// </summary>
        void StartAiAiGame();

        /// <summary>
        /// End local game.
        /// Set GameType, and SendGameStateChangedEvent to GameLogic and inform player in Platform GUI with GameEndedEvent.
        /// </summary>
        void EndLocalGame();

        /// <summary>
        /// Start AI-AI.
        /// Set GameType, and SendGameStateChangedEvent to GameLogic and inform player in Platform GUI with GameEndedEvent.
        /// </summary>
        void EndAiAiGame();

        /// <summary>
        /// Save local game if the game is started and the path with filename is correct.
        /// </summary>
        /// <param name="fileName">The filename of the saved data.</param>
        void SaveLocalGame(String fileName);

        /// <summary>
        /// Load local game if no game is started and the path with filename is correct.
        /// </summary>
        /// <param name="fileName">The filename of the load data.</param>
        void LoadLocalGame(String fileName);
        #endregion
    }
}
