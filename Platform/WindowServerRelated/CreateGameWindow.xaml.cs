using System;
using System.Linq;
using System.Windows;
using ConnectionInterface.MessageTypes;
using Platform.Model;

namespace Platform.WindowServerRelated
{
    /// <summary>
    /// Interaction logic for CreateGameWindow.xaml
    /// </summary>
    public partial class CreateGameWindow : Window
    {
        public CreateGameWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
        }

        private readonly NetworkManager _MNetworkManager;
        private readonly DataManager _MDataManager;

        public CreateGameWindow(NetworkManager networkManager, DataManager dataManager)
            : this()
        {
            _MNetworkManager = networkManager;
            _MDataManager = dataManager;

            GameSelectorComboBox.ItemsSource = dataManager.Games.Select(x => x.Type.Name);
        }

        private void CreateGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(GameSelectorComboBox.Text))
            {
                MessageBox.Show("Please select game from list first!", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            _MNetworkManager.CreateGame(_MDataManager.Games.First(x => x.Type.Name == GameSelectorComboBox.Text));
        }


    }
}
