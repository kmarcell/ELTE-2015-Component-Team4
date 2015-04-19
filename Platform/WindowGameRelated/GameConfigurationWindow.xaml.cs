using System.Linq;
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
            GameSelectorComboBox.ItemsSource = _MDataManager.Games.Select(x => x.Type.Name);
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(GameSelectorComboBox.Text))
            {
                MessageBox.Show("Please select game!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _MGameManager.StartGame(_MDataManager.Games.First(x => x.Type.Name == GameSelectorComboBox.Text));
        }
    }
}
