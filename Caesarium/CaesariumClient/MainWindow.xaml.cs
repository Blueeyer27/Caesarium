using CaesariumClient.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
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
