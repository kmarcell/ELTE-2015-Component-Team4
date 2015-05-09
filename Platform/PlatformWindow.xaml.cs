using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GTInterfacesLibrary;
using GTInterfacesLibrary.GameEvents;
using GUIImplementation;
using Platform.Model;
using Platform.WindowGameRelated;
using Platform.WindowServerRelated;
using GameEndedEventArgs = Platform.Events.EventsGameRelated.GameEndedEventArgs;
using GameEventArgs = Platform.Events.EventsServerRelated.GameEventArgs;

namespace Platform
{
    /// <summary>
    /// Interaction logic for PlatformWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region private fields
        private readonly List<MenuItem> _GuiListMenuItems = new List<MenuItem>();
        private readonly List<MenuItem> _GameListMenuItems = new List<MenuItem>();
        private readonly NetworkManager _MNetworkManager;
        private ServerConnectWindow _MServerConnectWindow;
        private ListGameWindow _MListGameWindow;
        private CreateGameWindow _MCreateGameWindow;

        private readonly GameManager _MGameManager;
        private GameConfigurationWindow _MGameConfigurationWindow;
        private Boolean _IsAiAiGameStarted;
        private GTGuiInterface _MCurrenGui;

        private const String LogfileName = "platform.log";
        private String LogMessagePrefix
        {
            get { return string.Format("{0} - ", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")); }
            
        }
        #endregion


        public MainWindow()
        {
            InitializeComponent();

            _IsAiAiGameStarted = false;

            // platform events
            Closing += OnClosing;
            
            CreateOnlineGameMenuItem.IsEnabled = false;
            ConnectOnlineGameMenuItem.IsEnabled = false;
            LeaveOnlineGameMenuItem.IsEnabled = false;
            MenuServerConnectMenuItem.IsEnabled = false;
            MenuServerDisconnectMenuItem.IsEnabled = false;
            SaveGameMenuItem.IsEnabled = false;
            LoadGameMenuItem.IsEnabled = false;
            EndGameMenuItem.IsEnabled = false;
            StartGameMenuItem.IsEnabled = false;
            StartAiAiGameMenuItem.IsEnabled = false;
            EndAiAiGameMenuItem.IsEnabled = false;
            GameMenuItem.IsEnabled = true;


            _MNetworkManager = new NetworkManager();
            _MNetworkManager.ConnectAcceptedEvent += NetworkManager_OnConnectAcceptedEvent;
            _MNetworkManager.ConnectRejectedServerNotRespondingEvent += NetworkManager_OnConnectRejectedServerNotRespondingEvent;
            _MNetworkManager.ConnectRejectedUsernameOccupied += NetworkManager_OnConnectRejectedUsernameOccupiedEvent;
            _MNetworkManager.DisconnectedEvent += NetworkManager_OnDisconnectedEvent;
            _MNetworkManager.GameCreatedEvent += MNetworkManager_OnGameCreatedEvent;
            _MNetworkManager.GameEndedEvent += MNetworkManager_OnGameEndedEvent;
            _MNetworkManager.GameCancelledEvent += MNetworkManager_OnGameCancelledEvent;
            _MNetworkManager.GameJoinAcceptedEvent += MNetworkManagerOnGameJoinAcceptedEvent;
            _MNetworkManager.GameJoinRejectedEvent += MNetworkManagerOnGameJoinRejectedEvent;
            _MNetworkManager.GameStatusReceived += MNetworkManager_OnGameStatusReceived;

            _MGameManager = new GameManager(_MNetworkManager);
            _MGameManager.GameStartedEvent += MGameManager_OnGameStartedEvent;
            _MGameManager.GameEndedEvent += MGameManager_OnGameEndedEvent;

            LoadGui();
            LoadGameLogic();
            LoadArtificalIntelligence();

            if (_MGameManager.GameLogicList.Any() && _MGameManager.GameGuiList.Any())
            {
                CreateOnlineGameMenuItem.IsEnabled = false;
                ConnectOnlineGameMenuItem.IsEnabled = false;
                LeaveOnlineGameMenuItem.IsEnabled = false;
                MenuServerConnectMenuItem.IsEnabled = true;
                MenuServerDisconnectMenuItem.IsEnabled = false;
                SaveGameMenuItem.IsEnabled = false;
                LoadGameMenuItem.IsEnabled = true;
                EndGameMenuItem.IsEnabled = false;
                StartGameMenuItem.IsEnabled = true;
                StartAiAiGameMenuItem.IsEnabled = true;
                EndAiAiGameMenuItem.IsEnabled = false;
            }
        }


        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (MessageBox.Show("Do you want to quit?", "Platform", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                cancelEventArgs.Cancel = true;
            }
        }


        #region Menuitem events
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void JoinMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _MServerConnectWindow = new ServerConnectWindow(_MNetworkManager);
            _MServerConnectWindow.ShowDialog();
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _MNetworkManager.Disconnect();
        }

        private void StartGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_MGameManager.CurrentGame == null)
            {
                MessageBox.Show("Load game before start game!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _MGameConfigurationWindow = new GameConfigurationWindow(_MGameManager);
            _MGameConfigurationWindow.ShowDialog();
        }

        private void EndGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _MGameManager.EndLocalGame();
        }

        private void CreateOnlineGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_MGameManager.CurrentGame == null)
            {
                MessageBox.Show("Load game before create to online game!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _MCreateGameWindow = new CreateGameWindow(_MNetworkManager, _MGameManager);
            _MCreateGameWindow.ShowDialog();
        }

        private void ConnectOnlineGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_MGameManager.CurrentGame == null)
            {
                MessageBox.Show("Load game before connect to online game!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _MListGameWindow = new ListGameWindow(_MNetworkManager, _MGameManager);
            _MListGameWindow.ShowDialog();
        }

        private void LeaveGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _MNetworkManager.EndGame();
        }

        private void LoadGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Load game",
                    Filter = "Gamesaves|*.compgame"
                };

                if(Convert.ToBoolean(openFileDialog.ShowDialog()))
                { 
                    _MGameManager.LoadLocalGame(openFileDialog.FileName);

                    PrintStatusBarMessage("Game loaded successfuly!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Loading game error!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Title = "Save game",
                    Filter = "Gamesaves|*.compgame"
                };


                if (Convert.ToBoolean(saveFileDialog.ShowDialog()))
                {
                    _MGameManager.SaveLocalGame(saveFileDialog.FileName);

                    PrintStatusBarMessage("Game saved successfuly!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Save game error!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartAiAiGameMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            _IsAiAiGameStarted = true;
            _MGameManager.StartAiAiGame();
        }

        private void EndAiAiGameMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            _IsAiAiGameStarted = false;
            _MGameManager.EndAiAiGame();
        }

        private void GuiMenuItemOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var headerName = ((MenuItem)routedEventArgs.Source).Header.ToString();

            _MCurrenGui = _MGameManager.GameGuiList.First(x => x.GuiName == headerName);
            GameContentControl.Content = _MCurrenGui;
            GameContentControl.Width = GameContentControl.Height = 500;
            _MGameManager.SetCurrentGui(_MCurrenGui);
            
            _GuiListMenuItems.ForEach(item =>
            {
                item.IsChecked = item.Header.ToString() == headerName;
            });
        }

        private void GameMenuItemOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var headerName = ((MenuItem)routedEventArgs.Source).Header.ToString();
            _MGameManager.SetCurrentGame(_MGameManager.GameLogicList.First(x => x.Name == headerName));
            _MGameManager.SetCurrentGui(_MCurrenGui);

            _GameListMenuItems.ForEach(item =>
            {
                item.IsChecked = item.Header.ToString() == headerName;
            });
        }
        #endregion


        #region Gamemanager events
        private void MGameManager_OnGameStartedEvent(object sender, EventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                PrintStatusBarMessage("Game started.");

                CreateOnlineGameMenuItem.IsEnabled = false;
                ConnectOnlineGameMenuItem.IsEnabled = false;
                LeaveOnlineGameMenuItem.IsEnabled = false;
                MenuServerConnectMenuItem.IsEnabled = false;
                MenuServerDisconnectMenuItem.IsEnabled = false;
                SaveGameMenuItem.IsEnabled = true;
                LoadGameMenuItem.IsEnabled = false;
                EndGameMenuItem.IsEnabled = !_IsAiAiGameStarted;
                StartGameMenuItem.IsEnabled = false;
                StartAiAiGameMenuItem.IsEnabled = false;
                EndAiAiGameMenuItem.IsEnabled = _IsAiAiGameStarted;
                GameMenuItem.IsEnabled = false;
            });
        }

        private void MGameManager_OnGameEndedEvent(object sender, GameEndedEventArgs eventArgs)
        {
            if (eventArgs.IsEnded && eventArgs.IsWin)
            {
                MessageBox.Show("Game finished. Congratulation you won!", "Platform", MessageBoxButton.OK, MessageBoxImage.Information);     
            }
            else if (eventArgs.IsEnded)
            {
                MessageBox.Show("Game finished. You lost!", "Platform", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Game canceled!", "Platform", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Dispatcher.Invoke(() =>
            {
                PrintStatusBarMessage("Game finished.");

                CreateOnlineGameMenuItem.IsEnabled = false;
                ConnectOnlineGameMenuItem.IsEnabled = false;
                LeaveOnlineGameMenuItem.IsEnabled = false;
                MenuServerConnectMenuItem.IsEnabled = true;
                MenuServerDisconnectMenuItem.IsEnabled = false;
                SaveGameMenuItem.IsEnabled = false;
                LoadGameMenuItem.IsEnabled = true;
                EndGameMenuItem.IsEnabled = false;
                StartGameMenuItem.IsEnabled = true;
                StartAiAiGameMenuItem.IsEnabled = true;
                EndAiAiGameMenuItem.IsEnabled = false;
                GameMenuItem.IsEnabled = true;
            });
        }
        #endregion


        #region Networkmanager events
        private void NetworkManager_OnConnectAcceptedEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                CreateOnlineGameMenuItem.IsEnabled = true;
                ConnectOnlineGameMenuItem.IsEnabled = true;
                LeaveOnlineGameMenuItem.IsEnabled = false;
                MenuServerConnectMenuItem.IsEnabled = false;
                MenuServerDisconnectMenuItem.IsEnabled = true;
                SaveGameMenuItem.IsEnabled = false;
                LoadGameMenuItem.IsEnabled = false;
                EndGameMenuItem.IsEnabled = false;
                StartGameMenuItem.IsEnabled = false;
                StartAiAiGameMenuItem.IsEnabled = false;
                EndAiAiGameMenuItem.IsEnabled = false;
                GameMenuItem.IsEnabled = false;

                _MServerConnectWindow.Close();
                PrintStatusBarMessage("Connect to the server successful!");
                MessageBox.Show("Connect to the server successful.", "Platform", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void NetworkManager_OnConnectRejectedServerNotRespondingEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Could not connect to the server, server not responding.", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private void NetworkManager_OnConnectRejectedUsernameOccupiedEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Could not connect to the server, please change username, because its already in use.", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private void MNetworkManager_OnGameStatusReceived(object sender, GameEventArgs gameEventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                if(gameEventArgs.Game.Phase != GamePhase.Ended)
                    PrintStatusBarMessage("Game changed, player turn: {0}.", gameEventArgs.Game.PlayerTurn);
            });
        }

        private void NetworkManager_OnDisconnectedEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                CreateOnlineGameMenuItem.IsEnabled = false;
                ConnectOnlineGameMenuItem.IsEnabled = false;
                LeaveOnlineGameMenuItem.IsEnabled = false;
                MenuServerConnectMenuItem.IsEnabled = true;
                MenuServerDisconnectMenuItem.IsEnabled = false;
                SaveGameMenuItem.IsEnabled = false;
                LoadGameMenuItem.IsEnabled = true;
                EndGameMenuItem.IsEnabled = false;
                StartGameMenuItem.IsEnabled = true;
                StartAiAiGameMenuItem.IsEnabled = true;
                EndAiAiGameMenuItem.IsEnabled = false;
                GameMenuItem.IsEnabled = true;

                PrintStatusBarMessage("You are disconnected from the server!");
                MessageBox.Show("You are disconnected from the server.", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            });
        }
        private void MNetworkManager_OnGameCreatedEvent(object sender, EventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
                {

                    CreateOnlineGameMenuItem.IsEnabled = false;
                    ConnectOnlineGameMenuItem.IsEnabled = false;
                    LeaveOnlineGameMenuItem.IsEnabled = true;
                    MenuServerConnectMenuItem.IsEnabled = false;
                    MenuServerDisconnectMenuItem.IsEnabled = true;
                    SaveGameMenuItem.IsEnabled = false;
                    LoadGameMenuItem.IsEnabled = false;
                    EndGameMenuItem.IsEnabled = false;
                    StartGameMenuItem.IsEnabled = false;
                    StartAiAiGameMenuItem.IsEnabled = false;
                    EndAiAiGameMenuItem.IsEnabled = false;
                    GameMenuItem.IsEnabled = false;

                    _MCreateGameWindow.Close();
                    PrintStatusBarMessage("Game created! Waiting for another player.");
                });
        }

        private void MNetworkManager_OnGameEndedEvent(object sender, GameEventArgs eventArgs)
        {
            var isWon = eventArgs.Game.Winner == _MNetworkManager.PlayerName;
            var message = isWon
                ? "Game finished! Gratulation you won!"
                : "Game finished! Sorry, you lost that round";
            MessageBox.Show(message, "Platform", MessageBoxButton.OK, MessageBoxImage.Information);

            Dispatcher.Invoke(() =>
            {
                PrintStatusBarMessage(message);
                CreateOnlineGameMenuItem.IsEnabled = true;
                ConnectOnlineGameMenuItem.IsEnabled = true;
                LeaveOnlineGameMenuItem.IsEnabled = false;
                MenuServerConnectMenuItem.IsEnabled = false;
                MenuServerDisconnectMenuItem.IsEnabled = true;
                SaveGameMenuItem.IsEnabled = false;
                LoadGameMenuItem.IsEnabled = false;
                EndGameMenuItem.IsEnabled = false;
                StartGameMenuItem.IsEnabled = false;
                StartAiAiGameMenuItem.IsEnabled = false;
                EndAiAiGameMenuItem.IsEnabled = false;
                GameMenuItem.IsEnabled = false;
            });
        }


        private void MNetworkManager_OnGameCancelledEvent(object sender, GameEventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                PrintStatusBarMessage("Game cancelled!");

                CreateOnlineGameMenuItem.IsEnabled = true;
                ConnectOnlineGameMenuItem.IsEnabled = true;
                LeaveOnlineGameMenuItem.IsEnabled = false;
                MenuServerConnectMenuItem.IsEnabled = false;
                MenuServerDisconnectMenuItem.IsEnabled = true;
                SaveGameMenuItem.IsEnabled = false;
                LoadGameMenuItem.IsEnabled = false;
                EndGameMenuItem.IsEnabled = false;
                StartGameMenuItem.IsEnabled = false;
                StartAiAiGameMenuItem.IsEnabled = false;
                EndAiAiGameMenuItem.IsEnabled = false;
                GameMenuItem.IsEnabled = false;
            });

            MessageBox.Show("Sorry, the game is cancelled.", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void MNetworkManagerOnGameJoinRejectedEvent(object sender, EventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                PrintStatusBarMessage("Join game rejected!");
            });
        }

        private void MNetworkManagerOnGameJoinAcceptedEvent(object sender, GameEventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                CreateOnlineGameMenuItem.IsEnabled = false;
                ConnectOnlineGameMenuItem.IsEnabled = false;
                LeaveOnlineGameMenuItem.IsEnabled = true;
                MenuServerConnectMenuItem.IsEnabled = false;
                MenuServerDisconnectMenuItem.IsEnabled = true;
                SaveGameMenuItem.IsEnabled = false;
                LoadGameMenuItem.IsEnabled = false;
                EndGameMenuItem.IsEnabled = false;
                StartGameMenuItem.IsEnabled = false;
                StartAiAiGameMenuItem.IsEnabled = false;
                EndAiAiGameMenuItem.IsEnabled = false;
                GameMenuItem.IsEnabled = false;

                PrintStatusBarMessage("Join game accepted, game could started, player turn: {0}!", eventArgs.Game.PlayerTurn);
            });
        }
        #endregion


        #region Load components
        private void LoadGameLogic()
        {
            const string gameLogicDirectory = @"Game";
            var currentDirectory = Directory.GetCurrentDirectory();
            var dirToSearch = Path.Combine(currentDirectory, gameLogicDirectory);

            if (!Directory.Exists(dirToSearch))
            {
                MessageBox.Show("GameLogic could not be loaded, directory does not exists, game not possible!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var gameLogicFromDirectory = Directory.GetFiles(gameLogicDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            if (!gameLogicFromDirectory.Any())
            {
                MessageBox.Show("GameLogic could not be loaded, GameLogic in the directory could not be found, game not possible!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            _MGameManager.InitializeGameLogic(gameLogicDirectory);

            foreach (var gtGameLogicInterface in _MGameManager.GameLogicList)
            {
                var menuItem = new MenuItem
                {
                    Header = gtGameLogicInterface.Name
                };
                menuItem.Click += GameMenuItemOnClick;

                GameMenuItem.Items.Add(menuItem);
                _GameListMenuItems.Add(menuItem);
            }

            if(_GameListMenuItems.Any())
                _GameListMenuItems.First().IsChecked = true;
            else
                PrintStatusBarMessage("GameLogic could not be loaded, GameLogic in the directory could not be found, game not possible!");
        }

        private void LoadGui()
        {
            //const string guiDirectory = @"Gui";
            //var currentDirectory = Directory.GetCurrentDirectory();
            //var dirToSearch = Path.Combine(currentDirectory, guiDirectory);

            //if (!Directory.Exists(dirToSearch))
            //{
            //    MessageBox.Show("Gui could not be loaded, directory does not exists, game not possible!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            //var guiFromDirectory = Directory.GetFiles(guiDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            //if (!guiFromDirectory.Any())
            //{
            //    MessageBox.Show("Gui could not be loaded, Gui in the directory could not be found, game not possible!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            //_MGameManager.InitializeGui(guiDirectory);

            var ligthGui = new LightGUI {Width = 500, Height = 500};
            var darkGui = new DarkGUI {Width = 500, Height = 500};
            ligthGui.InvalidateVisual();
            darkGui.InvalidateVisual();

            var guiList = new List<GTGuiInterface>
            {
                ligthGui,
                darkGui
            };
            
            _MGameManager.InitializeGui(guiList);
            GameContentControl.Content = guiList.First();
            GameContentControl.Width = GameContentControl.Height = 500;
            _MCurrenGui = guiList.First();
            
            foreach (var gameGui in _MGameManager.GameGuiList)
            {
                var menuItem = new MenuItem
                {
                    Header = gameGui.GuiName
                };
                menuItem.Click += GuiMenuItemOnClick;

                GuiMenuItem.Items.Add(menuItem);
                _GuiListMenuItems.Add(menuItem);
            }

            _GuiListMenuItems.First().IsChecked = true;
        }

        private void LoadArtificalIntelligence()
        {
            const string aiDirectory = @"ArtificialIntelligence";
            if (!Directory.Exists(aiDirectory))
            {
                PrintStatusBarMessage("AI could not be loaded, directory does not exists, local game not possible");
                return;
            }

            var aiFromDirectory = Directory.GetFiles(aiDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            if (!aiFromDirectory.Any())
            {
                PrintStatusBarMessage("AI could not be loaded, AI in the directory could not be found, local game not possible");
                return;
            }

            _MGameManager.InitializeArtificialIntelligence(aiDirectory);
        }
        #endregion


        #region log/message player
        private void PrintStatusBarMessage(string message, params object[] messageParams)
        {
            var messageToLog = string.Format("{0} {1}", LogMessagePrefix, string.Format(message, messageParams));
            MainStatusBarTextBlock.Text = messageToLog;
            WriteLog(message, messageParams);
        }

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
