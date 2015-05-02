using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using ConnectionInterface;
using Platform.Model;
using Platform.WindowGameRelated;
using Platform.WindowServerRelated;
using PlatformInterface.EventsGameRelated;
using PlatformInterface.EventsServerRelated;

namespace Platform
{
    /// <summary>
    /// Interaction logic for PlatformWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NetworkManager _MNetworkManager;
        private ServerConnectWindow _MServerConnectWindow;
        private ListGameWindow _MListGameWindow;
        private CreateGameWindow _MCreateGameWindow;

        private readonly GameManager _MGameManager;
        private GameConfigurationWindow _MGameConfigurationWindow;

        public MainWindow()
        {
            InitializeComponent();


            // platform events
            Closing += OnClosing;
            //TODO
            //CreateOnlineGameMenuItem.IsEnabled = false;
            //ConnectOnlineGameMenuItem.IsEnabled = false;
            LeaveOnlineGameMenuItem.IsEnabled = false;


            MenuServerConnectMenuItem.IsEnabled = true;
            MenuServerDisconnectMenuItem.IsEnabled = false;
            SaveGameMenuItem.IsEnabled = false;
            LoadGameMenuItem.IsEnabled = true;
            EndGameMenuItem.IsEnabled = false;
            StartGameMenuItem.IsEnabled = true;
            LoadGameLogicComponentMenuItem.IsEnabled = true;


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



            _MGameManager = new GameManager(_MNetworkManager);
            _MGameManager.GameStartedEvent += MGameManager_OnGameStartedEvent;
            _MGameManager.GameEndedEvent += MGameManager_OnGameEndedEvent;

            //TODO
            //LoadArtificalIntelligence();
        }

        private void MNetworkManagerOnGameJoinRejectedEvent(object sender, EventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                MainStatusBarTextBlock.Text = "Join game rejected!";
            });
        }

        private void MNetworkManagerOnGameJoinAcceptedEvent(object sender, EventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                MainStatusBarTextBlock.Text = "Join game accepted, game could started!";
            });
        }

        private void LoadArtificalIntelligence()
        {
            const string aiDirectory = "ArtificialIntelligence";

            foreach (var aiDll in Directory.GetFiles(aiDirectory, "*.dll", SearchOption.TopDirectoryOnly))
            {
                var aiAssembly = Assembly.LoadFrom(aiDll);
                Type aiType;

                try
                {
                    aiType = aiAssembly.GetTypes().FirstOrDefault(x => x.GetInterface("IArtificialIntelligence") != null);
                }
                catch (ReflectionTypeLoadException)
                {
                    aiType = null;
                }

                if (aiType == null)
                {
                    break;
                }

                var aiObject = Activator.CreateInstance(aiType);
                _MGameManager.RegisterArtificialIntelligence((IArtificialIntelligence)aiObject);
            }
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (MessageBox.Show("Do you want to quit?", "Platform", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                cancelEventArgs.Cancel = true;
            }
        }

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
            if (GameManager.CurrentGame == null)
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
            if (GameManager.CurrentGame == null)
            {
                MessageBox.Show("Load game before create to online game!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _MCreateGameWindow = new CreateGameWindow(_MNetworkManager);
            _MCreateGameWindow.ShowDialog();
        }

        private void ConnectOnlineGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.CurrentGame == null)
            {
                MessageBox.Show("Load game before connect to online game!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _MListGameWindow = new ListGameWindow(_MNetworkManager);
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

                openFileDialog.ShowDialog();
                _MGameManager.LoadLocalGame(openFileDialog.FileName);

                MainStatusBarTextBlock.Text = "Game loaded!";
                MessageBox.Show("Game loaded!", "Platform", MessageBoxButton.OK, MessageBoxImage.Information);
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

                saveFileDialog.ShowDialog();
                _MGameManager.SaveLocalGame(saveFileDialog.FileName);

                MainStatusBarTextBlock.Text = "Game saved!";
                MessageBox.Show("Game saved!", "Platform", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Save game error!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        #region Gamemanager events
        private void MGameManager_OnGameStartedEvent(object sender, EventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                SaveGameMenuItem.IsEnabled = true;
                LoadGameMenuItem.IsEnabled = false;
                EndGameMenuItem.IsEnabled = true;
                StartGameMenuItem.IsEnabled = false;
                MenuServerConnectMenuItem.IsEnabled = false;
                LoadGameLogicComponentMenuItem.IsEnabled = false;
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


            SaveGameMenuItem.IsEnabled = false;
            LoadGameMenuItem.IsEnabled = true;
            EndGameMenuItem.IsEnabled = false;
            StartGameMenuItem.IsEnabled = true;
            MenuServerConnectMenuItem.IsEnabled = true;
            LoadGameLogicComponentMenuItem.IsEnabled = true;
        }
        #endregion


        #region Networkmanager events

        private void NetworkManager_OnConnectAcceptedEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _MServerConnectWindow.Close();

                CreateOnlineGameMenuItem.IsEnabled = true;
                ConnectOnlineGameMenuItem.IsEnabled = true;
                LeaveOnlineGameMenuItem.IsEnabled = true;
                MenuServerDisconnectMenuItem.IsEnabled = true;
                MenuServerConnectMenuItem.IsEnabled = false;

                SaveGameMenuItem.IsEnabled = false;
                LoadGameMenuItem.IsEnabled = false;
                EndGameMenuItem.IsEnabled = false;
                StartGameMenuItem.IsEnabled = false;

                LoadGameLogicComponentMenuItem.IsEnabled = false;

                MainStatusBarTextBlock.Text = "Connect to the server successful!";
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

        private void NetworkManager_OnDisconnectedEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                CreateOnlineGameMenuItem.IsEnabled = false;
                ConnectOnlineGameMenuItem.IsEnabled = false;
                LeaveOnlineGameMenuItem.IsEnabled = false;
                MenuServerDisconnectMenuItem.IsEnabled = false;
                MenuServerConnectMenuItem.IsEnabled = true;

                SaveGameMenuItem.IsEnabled = false;
                LoadGameMenuItem.IsEnabled = true;
                EndGameMenuItem.IsEnabled = false;
                StartGameMenuItem.IsEnabled = true;

                LoadGameLogicComponentMenuItem.IsEnabled = true;

                MainStatusBarTextBlock.Text = "You are disconnected from the server!";
                MessageBox.Show("You are disconnected from the server.", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            });
        }
        private void MNetworkManager_OnGameCreatedEvent(object sender, EventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
                {
                    _MCreateGameWindow.Close();
                    MainStatusBarTextBlock.Text = "Game created! Waiting for another player.";
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
                CreateOnlineGameMenuItem.IsEnabled = true;
                ConnectOnlineGameMenuItem.IsEnabled = true;
                LeaveOnlineGameMenuItem.IsEnabled = false;
                MenuServerDisconnectMenuItem.IsEnabled = true;
                MenuServerConnectMenuItem.IsEnabled = false;
            });
        }


        private void MNetworkManager_OnGameCancelledEvent(object sender, GameEventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                MainStatusBarTextBlock.Text = "Game cancelled!";

                CreateOnlineGameMenuItem.IsEnabled = true;
                ConnectOnlineGameMenuItem.IsEnabled = true;
                LeaveOnlineGameMenuItem.IsEnabled = false;
                MenuServerDisconnectMenuItem.IsEnabled = true;
                MenuServerConnectMenuItem.IsEnabled = false;
            });

            MessageBox.Show("Sorry, the game is cancelled.", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        #endregion

        private void RegisterGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Register game",
                    Filter = "dll|*.dll"
                };

                openFileDialog.ShowDialog();
                var gameAssembly = Assembly.LoadFrom(openFileDialog.FileName);
                Type gameType;

                try
                {
                    gameType = gameAssembly.GetTypes().FirstOrDefault(x => x.GetInterface("IGame") != null);
                }
                catch (ReflectionTypeLoadException)
                {
                    gameType = null;
                }

                if (gameType == null)
                {
                    MessageBox.Show("Registering game error!\nPlease select sufficient game to play!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var obj = Activator.CreateInstance(gameType);
                _MGameManager.RegisterGame((IGame)obj);
                //using (Stream dllStream = new FileStream(openFileDialog.FileName, FileMode.Open))
                //{
                //    var hash = SHA1.Create().ComputeHash(dllStream);
                //}


                MainStatusBarTextBlock.Text = "Game registered!";
                MessageBox.Show("Game registered!", "Platform", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Registering game error!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
