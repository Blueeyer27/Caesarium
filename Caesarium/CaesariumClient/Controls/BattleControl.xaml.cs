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

namespace CaesariumClient.Controls
{
    /// <summary>
    /// Interaction logic for BattleControl.xaml
    /// </summary>
    public partial class BattleControl : UserControl
    {
        public BattleControl()
        {
            InitializeComponent();
        }

        private void battleFieldGrid_Loaded(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < 20; i++)
            {
                var colDef = new ColumnDefinition();
                colDef.Width = new GridLength(1, GridUnitType.Star);
                battleFieldGrid.ColumnDefinitions.Add(colDef);
            }

            for (var i = 0; i < 12; i++)
            {
                var rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                battleFieldGrid.RowDefinitions.Add(rowDef);
            }

            //battleFieldGrid.Background = new ImageBrush((BitmapImage)CreateImage().Source);

            for (var i = 0; i < 20; i++)
                for (var j = 0; j < 12; j++)
                {
                    var grassImg = CreateImage();


                    Grid.SetColumn(grassImg, i);
                    Grid.SetRow(grassImg, j);

                    battleFieldGrid.Children.Add(grassImg);
                }

            //battleFieldGrid.ShowGridLines = true;
            //battleFieldGrid.ColumnDefinitions.Add(new ColumnDefinition().);
        }

        private Image CreateImage()
        {
            Image Mole = new Image();
            ImageSource MoleImage = new BitmapImage(new Uri(@"C:\Users\Anton\Source\GitRepos\Caesarium_Main\Caesarium\CaesariumClient\Images\Textures\grass-texture.jpg", UriKind.Absolute));
            Mole.Source = MoleImage;
            return Mole;
        }
    }
}
