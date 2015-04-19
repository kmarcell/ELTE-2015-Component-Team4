using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ConnectionInterface.MessageTypes;
using Platform.EventsServerRelated;
using Platform.Model;

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
        private readonly DataManager _MDataManager;

        public ListGameWindow(NetworkManager networkManager, DataManager dataManager)
            : this()
        {
            _MNetworkManager = networkManager;
            _MDataManager = dataManager;
            _MNetworkManager.OnlineGamesReceived += MNetworkManager_OnOnlineGamesReceived;

            GameSelectorComboBox.ItemsSource = _MDataManager.Games.Select(x => x.Type.Name);
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
                _MNetworkManager.JoinGame(_MGames[OnlineGamesDataGrid.SelectedIndex].GameId);
                Close();
                return;
            }

            MessageBox.Show("Please select game from list first!", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }


        private void GameSelectorComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _MNetworkManager.GetOnlineGames(_MDataManager.Games[GameSelectorComboBox.SelectedIndex].Type.Id);
        }
    }
}
