using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CaesariumClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        const int port = 6112;
        const string address = "127.0.0.1";

        public LoginWindow()
        {
            InitializeComponent();
            ServerConnect.client = new TcpClient();
            ServerConnect.client.Connect(new IPEndPoint(IPAddress.Parse(address), port));
            ServerConnect.stream = ServerConnect.client.GetStream();
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var login = loginTextBox.Text;
                var password = passwordBox.Password;

                byte[] data = Encoding.Unicode.GetBytes("log:" + login + "/" + password);
                ServerConnect.stream.Write(data, 0, data.Length);

                var answer = "";
                do
                {
                    answer = ServerConnect.ReadServerAnswer();
                } while (answer == "");

                var clientID = int.Parse(answer);

                if (clientID < 1)
                    MessageBox.Show("Incorrect password");
                else
                {
                    ServerConnect.clientID = clientID;
                    MainWindow window = new MainWindow();
                    window.Show();
                    this.Close();
                }
            }
        }


    }
}
