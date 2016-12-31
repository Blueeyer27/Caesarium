using CaesariumClient.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaesariumClient
{
    public static class ServerConnect
    {
        public static TcpClient client;
        public static NetworkStream stream;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<String, Control> controls;
        const int port = 20012;
        const string address = "127.0.0.1";

        public MainWindow()
        {
            InitializeComponent();
            InitializeControls();

            ServerConnect.client = new TcpClient(address, port);
            ServerConnect.stream = ServerConnect.client.GetStream();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;

            if (controls.ContainsKey(item.Name))
            {
                contentControl.Content = controls[item.Name];

                if (item.Name != "AppControlItem")
                    this.KeyDown -= ((BattleControl)controls["AppControlItem"]).contentControl_KeyDown;
            }
        }

        private void InitializeControls()
        {
            controls = new Dictionary<String, Control>();
            controls.Add("MainControlItem", new MainControl());
            controls.Add("LoginControlItem", new LoginControl());
            //controls.Add("AppControlItem", new AppsControl());
            controls.Add("AppControlItem", new BattleControl());
            controls.Add("StoreControlItem", new StoreControl());
        }
    }
}
