using System;
using System.ComponentModel;
using System.Windows;
using Platform.Events;
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

        public MainWindow()
        {
            InitializeComponent();


            // platform events
            Closing += OnClosing;

            OnlineGameMenuItem.IsEnabled = false;
            MenuServerDisconnectMenuItem.IsEnabled = false;
            GameMenuItem.IsEnabled = true;


            _MGameManager = new GameManager();

            _MNetworkManager = new NetworkManager();
            _MNetworkManager.ConnectionChangedEvent += NetworkManager_ConnectionChanged;
            _MNetworkManager.ConnectAcceptedEvent += NetworkManager_ConnectAccepted;
            _MNetworkManager.ConnectRejectedServerNotRespondingEvent += NetworkManagerConnectRejectedServerNotResponding;
            _MNetworkManager.ConnectRejectedUsernameOccupied += NetworkManagerConnectRejectedUsernameOccupied;
            _MNetworkManager.DisconnectedEvent += NetworkManager_Disconnected;
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
            var gameConfigurationWindow = new GameConfigurationWindow();
            gameConfigurationWindow.ShowDialog();
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

                if (saveFileDialog.ShowDialog() == true)
                {
                    _MGameManager.SaveGame(saveFileDialog.FileName);
                }


                MessageBox.Show("Game saved!", "Platform", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Save game error!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        #region Networkmanager events
        private void NetworkManager_ConnectionChanged(object sender, ConnectionChangeEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.IsConnected)
                {
                    OnlineGameMenuItem.IsEnabled = true;
                    MenuServerDisconnectMenuItem.IsEnabled = true;
                    MenuServerConnectMenuItem.IsEnabled = false;
                    GameMenuItem.IsEnabled = false;
                }
                else
                {
                    OnlineGameMenuItem.IsEnabled = false;
                    MenuServerDisconnectMenuItem.IsEnabled = false;
                    MenuServerConnectMenuItem.IsEnabled = true;
                    GameMenuItem.IsEnabled = true;
                }
            });
        }

        private void NetworkManager_ConnectAccepted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _MServerConnectWindow.Hide();
                MessageBox.Show("Connect to the server successful.", "Platform", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void NetworkManagerConnectRejectedServerNotResponding(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Could not connect to the server, server not responding.", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private void NetworkManagerConnectRejectedUsernameOccupied(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Could not connect to the server, please change username, because its already in use.", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private void NetworkManager_Disconnected(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("You are successfuly disconnected from the server.", "Platform", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }
        #endregion


    }
}
