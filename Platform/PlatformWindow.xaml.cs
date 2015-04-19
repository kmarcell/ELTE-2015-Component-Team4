using System;
using System.ComponentModel;
using System.Windows;
using Platform.EventsGameRelated;
using Platform.EventsServerRelated;
using Platform.Model;
using Platform.WindowGameRelated;
using Platform.WindowServerRelated;

namespace Platform
{
    /// <summary>
    /// Interaction logic for PlatformWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GameManager _MGameManager;
        private readonly NetworkManager _MNetworkManager;
        private ServerConnectWindow _MServerConnectWindow;
        private GameConfigurationWindow _MgGameConfigurationWindow;
        private Boolean IsConnectedToServer;

        public MainWindow()
        {
            InitializeComponent();


            // platform events
            Closing += OnClosing;

            IsConnectedToServer = false;
            CreateOnlineGameMenuItem.IsEnabled = false;
            ConnectOnlineGameMenuItem.IsEnabled = false;
            LeaveOnlineGameMenuItem.IsEnabled = false;


            MenuServerConnectMenuItem.IsEnabled = true;
            MenuServerDisconnectMenuItem.IsEnabled = false;
            SaveGameMenuItem.IsEnabled = false;
            LoadGameMenuItem.IsEnabled = true;
            EndGameMenuItem.IsEnabled = false;
            StartGameMenuItem.IsEnabled = true;
            LoadAiComponentMenuItem.IsEnabled = true;
            LoadGameLogicComponentMenuItem.IsEnabled = true;



            _MGameManager = new GameManager();
            _MGameManager.GameStartedEvent += MGameManager_OnGameStartedEvent;
            _MGameManager.GameEndedEvent += MGameManager_OnGameEndedEvent;


            _MNetworkManager = new NetworkManager();
            _MNetworkManager.ConnectionChangedEvent += NetworkManager_OnConnectionChangedEvent;
            _MNetworkManager.ConnectAcceptedEvent += NetworkManager_OnConnectAcceptedEvent;
            _MNetworkManager.ConnectRejectedServerNotRespondingEvent += NetworkManager_OnConnectRejectedServerNotRespondingEvent;
            _MNetworkManager.ConnectRejectedUsernameOccupied += NetworkManager_OnConnectRejectedUsernameOccupiedEvent;
            _MNetworkManager.DisconnectedEvent += NetworkManager_OnDisconnectedEvent;
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
            _MServerConnectWindow = new ServerConnectWindow(networkManager: _MNetworkManager);
            _MServerConnectWindow.ShowDialog();
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _MNetworkManager.Disconnect();
        }

        private void StartGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _MgGameConfigurationWindow = new GameConfigurationWindow(_MGameManager);
            _MgGameConfigurationWindow.ShowDialog();
        }

        private void EndGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _MGameManager.EndGame();
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
                _MGameManager.LoadGame(openFileDialog.FileName);

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
                _MGameManager.SaveGame(saveFileDialog.FileName);

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
                _MgGameConfigurationWindow.Close();
                
                SaveGameMenuItem.IsEnabled = true;
                LoadGameMenuItem.IsEnabled = false;
                EndGameMenuItem.IsEnabled = true;
                StartGameMenuItem.IsEnabled = false;
                MenuServerConnectMenuItem.IsEnabled = false;
                LoadAiComponentMenuItem.IsEnabled = false;
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
            LoadAiComponentMenuItem.IsEnabled = true;
            LoadGameLogicComponentMenuItem.IsEnabled = true;
        }
        #endregion


        #region Networkmanager events
        private void NetworkManager_OnConnectionChangedEvent(object sender, ConnectionChangeEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.IsConnected)
                {
                    CreateOnlineGameMenuItem.IsEnabled = true;
                    ConnectOnlineGameMenuItem.IsEnabled = true;
                    LeaveOnlineGameMenuItem.IsEnabled = true;
                    MenuServerDisconnectMenuItem.IsEnabled = true;
                    MenuServerConnectMenuItem.IsEnabled = false;

                    SaveGameMenuItem.IsEnabled = false;
                    LoadGameMenuItem.IsEnabled = false;
                    EndGameMenuItem.IsEnabled = false;
                    StartGameMenuItem.IsEnabled = false;

                    LoadAiComponentMenuItem.IsEnabled = false;
                    LoadGameLogicComponentMenuItem.IsEnabled = false;
                }
                else
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

                    LoadAiComponentMenuItem.IsEnabled = true;
                    LoadGameLogicComponentMenuItem.IsEnabled = true;

                    if (IsConnectedToServer)
                    {
                        IsConnectedToServer = false;
                        MessageBox.Show("Unkown error!\nYou are disconnected from the server.", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            });
        }

        private void NetworkManager_OnConnectAcceptedEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _MServerConnectWindow.Close();
                IsConnectedToServer = true;
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
                MessageBox.Show("You are successfuly disconnected from the server.", "Platform", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }
        #endregion


    }
}
