using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CaesariumClient.Controls
{
    public partial class BattleControl : UserControl
    {
        private void InitializeBattleField()
        {
            for (var i = 0; i < 30; i++)
            {
                var colDef = new ColumnDefinition();
                colDef.Width = new GridLength(1, GridUnitType.Star);
                battleFieldGrid.ColumnDefinitions.Add(colDef);
            }

            for (var i = 0; i < 18; i++)
            {
                var rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                battleFieldGrid.RowDefinitions.Add(rowDef);
            }

            //battleFieldGrid.Background = new ImageBrush((BitmapImage)CreateImage().Source);
            //Image img = CreateImage();

            for (var i = 0; i < 30; i++)
                for (var j = 0; j < 18; j++)
                {
                    var grassImg = CreateObjectImage(@"\Images\Textures\grass-texture.jpg");

                    Grid.SetColumn(grassImg, i);
                    Grid.SetRow(grassImg, j);

                    battleFieldGrid.Children.Add(grassImg);
                }

            //battleFieldGrid.ShowGridLines = true;
            //battleFieldGrid.ColumnDefinitions.Add(new ColumnDefinition().);
        }
    }
}
