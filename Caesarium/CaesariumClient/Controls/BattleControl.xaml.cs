using System;
using System.Collections.Generic;
using System.IO;
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

namespace CaesariumClient.Controls
{
    /// <summary>
    /// Interaction logic for BattleControl.xaml
    /// </summary>
    public partial class BattleControl : UserControl
    {
        Image player;
        int x = 1, y = 1;

        UserControl objectsField;
        public BattleControl()
        {
            InitializeComponent();
            InitializeBattleField();
            InitializeBattleObjects();

            player = CreateObjectImage(@"\Images\Objects\admin.gif");

            //player.Stretch = Stretch.None;

            AddBattleObject(x, y, player);
        }

        private void battleFieldGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private Image CreateObjectImage(string uri)
        {
            var dir = Directory.GetCurrentDirectory();
            Image Mole = new Image();
            ImageSource MoleImage = new BitmapImage(new Uri(uri, UriKind.Relative));
            Mole.Source = MoleImage;
            
            return Mole;
        }

        private void contentControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += contentControl_KeyDown;
        }

        //string moves = "";
        public void contentControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                //moves += "w";
                MoveBattleObject(x, --y, player);
            }
            if (e.Key == Key.S)
            {
                MoveBattleObject(x, ++y, player);
            }
            if (e.Key == Key.D)
            {
                //player.Source = new BitmapImage(new Uri(@"\Images\Textures\grass-texture.jpg", UriKind.Relative));
                MoveBattleObject(++x, y, player);
            }
            if (e.Key == Key.A)
            {
                //player.Source = new BitmapImage(new Uri(@"\Images\Objects\admin.gif", UriKind.Relative));
                MoveBattleObject(--x, y, player);
            }
        }
    }
}
