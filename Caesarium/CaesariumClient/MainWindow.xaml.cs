using CaesariumClient.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaesariumClient
{
    public static class ServerConnect
    {
        public static TcpClient client;
        public static NetworkStream stream;

        public static int clientID;

        public static string ReadServerAnswer()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder("");
            int bytes = 0;

            while (stream.DataAvailable)
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }

            return builder.ToString().Trim();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<String, Control> controls;

        public MainWindow()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;

            if (controls.ContainsKey(item.Name))
            {
                contentControl.Content = controls[item.Name];

                if (item.Name != "AppControlItem")
                    this.KeyDown -= ((BattleControl)controls["AppControlItem"]).contentControl_KeyDown;
                else
                {
                    byte[] data = Encoding.Unicode.GetBytes("startGame:0");
                    ServerConnect.stream.Write(data, 0, data.Length);
                }
            }
        }

        private void InitializeControls()
        {
            controls = new Dictionary<String, Control>();
            controls.Add("MainControlItem", new MainControl());
            //controls.Add("AppControlItem", new AppsControl());
            controls.Add("AppControlItem", new BattleControl());
            controls.Add("StoreControlItem", new StoreControl());
        }
    }
}
