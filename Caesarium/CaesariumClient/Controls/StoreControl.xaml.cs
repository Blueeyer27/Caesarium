using CaesariumClient.Controls.Store;
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
    /// Interaction logic for StoreControl.xaml
    /// </summary>
    public partial class StoreControl : UserControl
    {
        List<Item> storeItems;

        public StoreControl()
        {
            InitializeComponent();
            storeItems = LoadItems();

            for (var i = 0; i < storeItems.Count; i++)
            {
                var border = new Border()
                {
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    BorderThickness = new Thickness(2)
                };
                
                var grid = CreateProductGrid(1, 4, 125, 125);
                AddItemToGrid(grid, storeItems[i]);

                border.Child = grid;
                shopListView.Items.Add(border);
            }
        }

        public void AddItemToGrid(Grid grid, Item item) 
        {
            var imgLabel = item.ImageLabel;
            imgLabel.Height = 100;
            imgLabel.Width = 100;
            Grid.SetColumn(imgLabel, 0);
            grid.Children.Add(imgLabel);

            var nameLabel = new Label()
            {
                Content = item.Name,
                Foreground = new SolidColorBrush(Color.FromRgb(1, 1, 1)),
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Italic,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(nameLabel, 1);
            grid.Children.Add(nameLabel);

            string stats = "";
            stats += "Hp: " + item.Hp + "\n";
            stats += "Power: " + item.Power + "\n";
            stats += "Defence: " + item.Defence + "\n\n\n";
            stats += "Gold: " + 999 + "";

            var statsLabel = new Label()
            {
                Content = stats,
                Foreground = new SolidColorBrush(Color.FromRgb(1, 1, 1)),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Italic,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(statsLabel, 2);
            grid.Children.Add(statsLabel);

            var purchTextLabel = new Label()
            {
                Content = "Purchase",
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(10, 0, 0, 10),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Bottom
            };

            Grid.SetColumn(purchTextLabel, 3);
            grid.Children.Add(purchTextLabel);


            var purchaseLabel = new Label()
            {
                Width = 80,
                Height = 80,
                Margin = new Thickness(20, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            purchaseLabel.Style = (Style)Resources["chestLabelStyle"];
            Grid.SetColumn(purchaseLabel, 3);
            grid.Children.Add(purchaseLabel);
        }

        public Grid CreateProductGrid(int rows, int cols, int width, int height)
        {
            Grid grid;

            grid = new Grid() { };
            grid.Height = rows * height;
            grid.Width = cols * width + 75;

            for (var i = 0; i < rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(height)
                });
            }
            for (var i = 0; i < cols; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(i == 1 ? 200 : width)
                });
            }



            return grid;
        }

        public List<Item> LoadItems()
        {
            var itemList = new List<Item>();
            for (var i = 1; i < 6; i++)
            {
                itemList.Add(new Item(i, "Infinity Staff" + i, "staff" + i + ".png", i, i + 1, i + 2));
            }

            for (var i = 1; i < 5; i++)
            {
                itemList.Add(new Item(i, "Boots of Asgard" + i, "boots" + i + ".png", i, i + 1, i + 2));
            }

            for (var i = 1; i < 5; i++)
            {
                itemList.Add(new Item(i, "Destorus Robe" + i, "robe" + i + ".png", i, i + 1, i + 2));
            }

            return itemList;
        }
    }
}
