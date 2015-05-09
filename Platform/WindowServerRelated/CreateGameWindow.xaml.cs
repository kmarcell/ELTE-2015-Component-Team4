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
        private readonly GameManager _MGameManager;

        public CreateGameWindow(NetworkManager networkManager, GameManager gameManager)
            : this()
        {
            _MNetworkManager = networkManager;
            _MGameManager = gameManager;
            LoadedGameLabel.Content = _MGameManager.CurrentGame.Name;
        }

        private void CreateGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (_MGameManager.CurrentGame == null)
            {
                MessageBox.Show("Please load game first (File)!", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            _MNetworkManager.CreateGame(_MGameManager.CurrentGame);
        }
    }
}
