﻿using System.Collections.Generic;
using System.Windows;
using GTInterfacesLibrary;
using Platform.Model;

namespace Platform.WindowGameRelated
{
    /// <summary>
    /// Interaction logic for GameConfigurationWindow.xaml
    /// </summary>
    public partial class GameConfigurationWindow : Window
    {
        private readonly GameManager _MGameManager;
        private readonly List<IGTArtificialIntelligenceInterface> _MArtificialIntelligences; 
        public GameConfigurationWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
        }

        public GameConfigurationWindow(GameManager gameManager) : this()
        {
            _MGameManager = gameManager;
            CurrentlyLoadedGameLabel.Content = _MGameManager.CurrentGame.Name;
            _MArtificialIntelligences = _MGameManager.ArtificialIntelligenceList;
            AiDataGrid.ItemsSource = _MGameManager.ArtificialIntelligenceList;
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (_MGameManager.CurrentGame == null)
            {
                MessageBox.Show("Please load game before play (File)!", "Platform", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (AiDataGrid.SelectedItems.Count > 0)
            {
                _MGameManager.StartLocalGame(_MArtificialIntelligences[AiDataGrid.SelectedIndex]);
                Close();
                return;
            }

            MessageBox.Show("Please select AI from list first!", "Platform", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
