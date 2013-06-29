using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace ServerLoader_Controller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket _clientSocket;
        private byte[] _buffer;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (_clientSocket != null && _clientSocket.Connected)
            {
                _clientSocket.Close();
            }
        }

        private void UpdateControlStates(bool val)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                btnConnect.IsEnabled = val;
                btnRestart.IsEnabled = !val;
                btnUpdate.IsEnabled = !val;
                btnSpells.IsEnabled = !val;
                btnCreatures.IsEnabled = !val;
                btnItems.IsEnabled = !val;
            }));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(IPBox.Text), 3333), new AsyncCallback(ConnectCallback), null);
                UpdateControlStates(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                UpdateControlStates(true);
            }
        }

        private void ConnectCallback(IAsyncResult AR)
        {
            try
            {
                _clientSocket.EndConnect(AR);
                UpdateControlStates(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                UpdateControlStates(true);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _buffer = Encoding.ASCII.GetBytes("User: map");
                _clientSocket.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }
            catch (SocketException) { MessageBox.Show("Server closed connection."); } // Server closed connection
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                UpdateControlStates(false);
            }
        }

        private void SendCallback(IAsyncResult AR)
        {
            _clientSocket.EndSend(AR);
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _buffer = Encoding.ASCII.GetBytes("User: restart");
                _clientSocket.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }
            catch (SocketException) { MessageBox.Show("Server closed connection."); } // Server closed connection
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                UpdateControlStates(false);
            }
        }

        private void btnSpells_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _buffer = Encoding.ASCII.GetBytes("User: spells");
                _clientSocket.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }
            catch (SocketException) { MessageBox.Show("Server closed connection."); } // Server closed connection
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                UpdateControlStates(false);
            }
        }

        private void btnItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _buffer = Encoding.ASCII.GetBytes("User: items");
                _clientSocket.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }
            catch (SocketException) { MessageBox.Show("Server closed connection."); } // Server closed connection
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                UpdateControlStates(false);
            }
        }

        private void btnCreatures_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _buffer = Encoding.ASCII.GetBytes("User: creatures");
                _clientSocket.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }
            catch (SocketException) { MessageBox.Show("Server closed connection."); } // Server closed connection
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
                UpdateControlStates(false);
            }
        }
    }
}
