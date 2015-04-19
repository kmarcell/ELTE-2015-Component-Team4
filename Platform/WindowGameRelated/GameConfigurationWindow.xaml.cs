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
        private readonly DataManager _MDataManager;

        public GameConfigurationWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
        }

        public GameConfigurationWindow(GameManager gameManager, DataManager dataManager) : this()
        {
            _MGameManager = gameManager;
            _MDataManager = dataManager;
            CurrentlyLoadedGameLabel.Content = _MDataManager.CurrentGame.Type.Name;
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (_MDataManager.CurrentGame == null)
            {
                MessageBox.Show("Please load game before play (File)!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _MGameManager.StartGame(_MDataManager.CurrentGame);
        }
    }
}
