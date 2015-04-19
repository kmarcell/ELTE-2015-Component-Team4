using System;
using System.Text.RegularExpressions;
using System.Windows;
using Platform.Model;

namespace Platform.WindowServerRelated
{
    /// <summary>
    /// Interaction logic for ServerConnectWindow.xaml
    /// </summary>
    public partial class ServerConnectWindow : Window
    {
        public ServerConnectWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
        }


        private readonly NetworkManager _MNetworkManager;

        public ServerConnectWindow(NetworkManager networkManager) : this()
        {
            _MNetworkManager = networkManager;
        }

        private readonly Regex _MServerIpAddressRegex = new Regex(@"^\d+\.\d+\.\d+\.\d+$", RegexOptions.Compiled);
        private readonly Regex _MServerPortNumberRegex = new Regex(@"^\d+$", RegexOptions.Compiled);
        private readonly Regex _MUsernameRegex = new Regex(@"^[a-zA-Z0-9]+$", RegexOptions.Compiled);

        public void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ServerIpAddressTextBox.Text))
            {
                MessageBox.Show("Server ip address is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(ServerPortNumberTextBox.Text))
            {
                MessageBox.Show("Server port number empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                MessageBox.Show("Username is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_MServerIpAddressRegex.IsMatch(ServerIpAddressTextBox.Text))
            {
                MessageBox.Show("Server ip address is incorrect formatted. Should be formatted: 192.168.0.1", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_MServerPortNumberRegex.IsMatch(ServerPortNumberTextBox.Text))
            {
                MessageBox.Show("Server port number is incorrect formatted. Should be a number: 5503", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_MUsernameRegex.IsMatch(UsernameTextBox.Text))
            {
                MessageBox.Show("Username is incorrect formatted. It can contains only chars: a-z, A-Z, 0-9", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _MNetworkManager.Connect(ServerIpAddressTextBox.Text, Convert.ToInt32(ServerPortNumberTextBox.Text), UsernameTextBox.Text);
        }
    }
}
