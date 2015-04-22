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

        public CreateGameWindow(NetworkManager networkManager)
            : this()
        {
            _MNetworkManager = networkManager;
            LoadedGameLabel.Content = GameManager.CurrentGame.Name;
        }

        private void CreateGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.CurrentGame == null)
            {
                MessageBox.Show("Please load game first (File)!", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            _MNetworkManager.CreateGame(GameManager.CurrentGame, GameManager.CurrentGame.GetHashCode());
        }
    }
}
