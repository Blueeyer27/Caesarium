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
    /// Interaction logic for CharactersControl.xaml
    /// </summary>
    public partial class CharactersControl : UserControl
    {
        public CharactersControl()
        {
            InitializeComponent();
            MakeInventory(3, 6, 70, 70);
            FillGrid(Inventory);
            this.Content = Inventory;
            this.MouseMove += new MouseEventHandler(Inventory_MouseMove);
        }

        private void Inventory_MouseMove(object sender, MouseEventArgs e)
        {
            if (onMove)
            {
                TT.X += Mouse.GetPosition(MoveLabel).X - 35;
                TT.Y += Mouse.GetPosition(MoveLabel).Y + 1;
            }
        }

        Grid Inventory;
        Label TmpLabel, MoveLabel = new Label();
        int tmpRow, tmpCol;
        bool onMove = false;
        TranslateTransform TT = new TranslateTransform();

        public Grid MakeInventory(int rows, int cols, int width, int height)
        {
            Inventory = new Grid();
            Inventory.Height = rows * height;
            Inventory.Width = cols * width;

            for (var i = 0; i < rows; i++)
            {
                Inventory.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(height)
                });
            }
            for (var i = 0; i < cols; i++)
            {
                Inventory.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(width)
                });
            }

            var appPath = Directory.GetCurrentDirectory();
            appPath = appPath.Substring(0, appPath.Length - 10);

            Inventory.Background = new ImageBrush(new BitmapImage(
                new Uri(appPath + "/Images/inventory_bg.jpg")));
            return Inventory;
        }

        public void FillGrid(Grid grid)
        {
            var appPath = Directory.GetCurrentDirectory();
            appPath = appPath.Substring(0, appPath.Length - 10);

            var images = new List<ImageBrush>();
            for (var i = 1; i < 6; i++)
            {
                ImageBrush item = new ImageBrush(new BitmapImage(
                    new Uri(appPath + "/Images/Items/staff" + i + ".png")));
                images.Add(item);
            }

            for (var i = 1; i < 5; i++)
            {
                ImageBrush item = new ImageBrush(new BitmapImage(
                    new Uri(appPath + "/Images/Items/boots" + i + ".png")));
                images.Add(item);
            }

            for (var i = 1; i < 5; i++)
            {
                ImageBrush item = new ImageBrush(new BitmapImage(
                    new Uri(appPath + "/Images/Items/robe" + i + ".png")));
                images.Add(item);
            }

            var rows = grid.RowDefinitions.Count;
            var cols = grid.ColumnDefinitions.Count;

            Label [,] items = new Label[rows, cols];
            Random R = new Random();
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    items[i, j] = new Label()
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(0.5),
                        Background = images[R.Next(images.Count)]
                    };

                    items[i, j].MouseLeftButtonDown += Inventory_MouseLeftButtonDown;
                    items[i, j].MouseLeftButtonUp += Inventory_MouseLeftButtonUp;

                    Grid.SetRow(items[i, j], i);
                    Grid.SetColumn(items[i, j], j);
                    grid.Children.Add(items[i, j]);
                }
            }
        }

        private void Inventory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TmpLabel == null)
            {
                onMove = !onMove;
                TmpLabel = (Label)sender;
                TmpLabel.Opacity = 0.5;
                tmpRow = Grid.GetRow((Label)sender);
                tmpCol = Grid.GetColumn((Label)sender);
                MoveLabel.Background = TmpLabel.Background;
                MoveLabel.RenderTransform = TT;
                Inventory.Children.Add(MoveLabel);
            }

            Inventory_MouseMove(null, e);
        }

        private void Inventory_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TmpLabel != null)
            {
                onMove = !onMove;
                Inventory.Children.Remove(TmpLabel);
                Grid.SetRow(TmpLabel, Grid.GetRow((Label)sender));
                Grid.SetColumn(TmpLabel, Grid.GetColumn((Label)sender));
                Inventory.Children.Remove((Label)sender);
                TmpLabel.Opacity = 1.0;
                Inventory.Children.Add(TmpLabel);
                Grid.SetRow((Label)sender, tmpRow);
                Grid.SetColumn((Label)sender, tmpCol);
                if (!Inventory.Children.Contains((Label)sender))
                    Inventory.Children.Add((Label)sender);
                else
                    ((Label)sender).Opacity = 1.0;

                TmpLabel = null;
                Inventory.Children.Remove(MoveLabel);
            }

            Inventory_MouseMove(null, e);
        }

        public void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TmpLabel != null)
            {
                onMove = !onMove;
                TmpLabel.Opacity = 1.0;

                TmpLabel = null;
                Inventory.Children.Remove(MoveLabel);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.MouseLeftButtonUp += UserControl_MouseLeftButtonUp;
        }
    }
}
