using System.Windows;
using Platform.Model;

namespace Platform.WindowGameRelated
{
    /// <summary>
    /// Interaction logic for GameConfigurationWindow.xaml
    /// </summary>
    public partial class GameConfigurationWindow : Window
    {
        private readonly GameManager _MGameManager;

        public GameConfigurationWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
        }

        public GameConfigurationWindow(GameManager gameManager) : this()
        {
            _MGameManager = gameManager;
            CurrentlyLoadedGameLabel.Content = DataManager.CurrentGame.Name;
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataManager.CurrentGame == null)
            {
                MessageBox.Show("Please load game before play (File)!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _MGameManager.StartGame();
        }
    }
}
