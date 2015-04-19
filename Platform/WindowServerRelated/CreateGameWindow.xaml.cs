using System.Linq;
using System.Windows;
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

            LoadedGameLabel.Content = _MDataManager.CurrentGame.Type.Name;
        }

        private void CreateGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (_MDataManager.CurrentGame == null)
            {
                MessageBox.Show("Please load game first (File)!", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            _MNetworkManager.CreateGame(_MDataManager.CurrentGame);
        }
    }
}
