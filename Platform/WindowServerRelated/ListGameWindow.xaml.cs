﻿using System.Windows;
using GTInterfacesLibrary.MessageTypes;
using Platform.Model;
using GamesEventArgs = Platform.Events.EventsServerRelated.GamesEventArgs;

namespace Platform.WindowServerRelated
{
    /// <summary>
    /// Interaction logic for ListGameWindow.xaml
    /// </summary>
    public partial class ListGameWindow : Window
    {
        public ListGameWindow()
        {
            InitializeComponent();
        }

        private Game[] _MGames;
        private readonly NetworkManager _MNetworkManager;
        private readonly GameManager _MGameManager;

        public ListGameWindow(NetworkManager networkManager, GameManager gameManager)
            : this()
        {
            _MNetworkManager = networkManager;
            _MGameManager = gameManager;
            _MNetworkManager.OnlineGamesReceived += MNetworkManager_OnOnlineGamesReceived;
            LoadedGameLabel.Content = _MGameManager.CurrentGame.Name;
            _MNetworkManager.GetOnlineGames(_MGameManager.CurrentGame);
            
        }

        private void MNetworkManager_OnOnlineGamesReceived(object sender, GamesEventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                OnlineGamesDataGrid.ItemsSource = eventArgs.Games;
                _MGames = eventArgs.Games;
            });
        }

        private void ConnectGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnlineGamesDataGrid.SelectedItems.Count > 0)
            {
                _MNetworkManager.JoinGame(_MGames[OnlineGamesDataGrid.SelectedIndex].Id);
                Close();
                return;
            }

            MessageBox.Show("Please select game from list first!", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
