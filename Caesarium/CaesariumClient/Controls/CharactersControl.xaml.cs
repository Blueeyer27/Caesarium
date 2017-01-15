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
        List<Grid> equipment = new List<Grid>();
        string appPath = Directory.GetCurrentDirectory();
            
        public CharactersControl()
        {
            InitializeComponent();
            appPath = appPath.Substring(0, appPath.Length - 10);
            //MakeInventory(3, 6, 55, 55);
            //FillGrid(Inventory);
            

            //this.Content = Inventory;
            //this.MouseMove += new MouseEventHandler(Inventory_MouseMove);
            equipment.Add(leftWeaponGrid);
            equipment.Add(leftArmorGrid);
            equipment.Add(leftBootsGrid);

            equipment.Add(rightWeaponGrid);
            equipment.Add(rightArmorGrid);
            equipment.Add(rightBootsGrid);

            foreach (var grid in equipment)
            {
                var bg = new Image();
                bg.Source = new BitmapImage(new Uri(appPath + "/Images/equip_bg.png"));
                bg.Stretch = Stretch.Fill;
                var label = new Label();
                Grid.SetRow(label, 0);
                Grid.SetColumn(label, 0);
                grid.Children.Add(bg);
                grid.Children.Add(label);

                label.MouseLeftButtonDown += Inventory_MouseLeftButtonDown;
                label.MouseLeftButtonUp += Inventory_MouseLeftButtonUp;
            }
        }

        private void ChangeZIndex(Grid except, int zIndex)
        {
            foreach (var grid in equipment)
            {
                if (grid != except)
                {
                    Panel.SetZIndex(grid, zIndex);
                }
            }
        }

        private void Inventory_MouseMove(object sender, MouseEventArgs e)
        {
            if (onMove)
            {
                TT.X += Mouse.GetPosition(MoveLabel).X - 27;
                TT.Y += Mouse.GetPosition(MoveLabel).Y + 1;
            }
        }

        Label TmpLabel, MoveLabel = new Label();
        Grid TmpGrid;
        int tmpRow, tmpCol;
        bool onMove = false;
        TranslateTransform TT = new TranslateTransform();

        public Grid MakeInventory(int rows, int cols, int width, int height)
        {
            Grid Inventory;

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

            return Inventory;
        }

        public void FillGrid(Grid grid)
        {
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

                    var bg = new Image();
                    bg.Source = new BitmapImage(new Uri(appPath + "/Images/equip_bg.png"));
                    bg.Stretch = Stretch.Fill;
                    Grid.SetRow(bg, i);
                    Grid.SetColumn(bg, j);
                    grid.Children.Add(bg);

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
                TmpGrid = GetChildrenGrid(TmpLabel);
                TmpLabel.Opacity = 0.5;
                tmpRow = Grid.GetRow((Label)sender);
                tmpCol = Grid.GetColumn((Label)sender);
                MoveLabel.Background = TmpLabel.Background;
                MoveLabel.RenderTransform = TT;
                ChangeZIndex(TmpGrid, -5);
                TmpGrid.Children.Add(MoveLabel);
            }

            Inventory_MouseMove(null, e);
        }

        private void Inventory_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TmpLabel != null)
            {
                onMove = !onMove;
                TmpGrid.Children.Remove(TmpLabel);
                Grid.SetRow(TmpLabel, Grid.GetRow((Label)sender));
                Grid.SetColumn(TmpLabel, Grid.GetColumn((Label)sender));

                var destGrid = GetChildrenGrid((Label)sender);
                if (destGrid != null)
                {
                    destGrid.Children.Remove((Label)sender);
                    destGrid.Children.Add(TmpLabel);
                }

                TmpLabel.Opacity = 1.0;
                Grid.SetRow((Label)sender, tmpRow);
                Grid.SetColumn((Label)sender, tmpCol);
                if (!TmpGrid.Children.Contains((Label)sender))
                    TmpGrid.Children.Add((Label)sender);
                else
                    ((Label)sender).Opacity = 1.0;

                TmpLabel = null;
                TmpGrid.Children.Remove(MoveLabel);
                ChangeZIndex(TmpGrid, 0);
                
            }

            Inventory_MouseMove(null, e);
        }


        private Grid GetChildrenGrid(Label elem)
        {
            foreach(var grid in equipment)
            {
                if (grid.Children.Contains(elem))
                    return grid;
            }

            return null;
        }

        public void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TmpLabel != null)
            {
                onMove = !onMove;
                TmpLabel.Opacity = 1.0;

                TmpLabel = null;
                TmpGrid.Children.Remove(MoveLabel);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.MouseLeftButtonUp += UserControl_MouseLeftButtonUp;
            window.MouseMove += Inventory_MouseMove;
        }

        Image leftChar = new Image();
        Image leftCharSprites = new Image();
        Image rightChar = new Image();
        Image rightCharSprites = new Image();
        int charPos = 0;
        private void inventoryBorder_Loaded(object sender, RoutedEventArgs e)
        {
            var inventory = MakeInventory(7, 6, 60, 60);
            FillGrid(inventory);
            inventory.VerticalAlignment = VerticalAlignment.Top;
            ((Border)sender).Child = inventory;

            equipment.Add(inventory);

            leftCharSprites.Source = new BitmapImage(new Uri(appPath + "/Images/Objects/skin2.png"));
            leftChar.Source = new CroppedBitmap(leftCharSprites.Source as BitmapSource,
                new Int32Rect(0 * 48, 0 * 48, 48, 48));
            Panel.SetZIndex(leftChar, Panel.GetZIndex(leftCharBorder));
            //leftCharBorder.Background = new ImageBrush(new BitmapImage(new Uri(appPath + "/Images/char_bg.png")));
            leftCharBorder.Child = leftChar;

            rightCharSprites.Source = new BitmapImage(new Uri(appPath + "/Images/Objects/skin5.png"));
            rightChar.Source = new CroppedBitmap(rightCharSprites.Source as BitmapSource,
                new Int32Rect(0 * 48, 0 * 48, 48, 48));
            Panel.SetZIndex(rightChar, Panel.GetZIndex(rightCharBorder));
            //rightCharBorder.Background = new ImageBrush(new BitmapImage(new Uri(appPath + "/Images/char_bg.png")));
            rightCharBorder.Child = rightChar;


            CreateTimer(AnimatePlayers, 120);
        }

        private void AnimatePlayers(object sender, EventArgs args)
        {
            leftChar.Source = new CroppedBitmap(leftCharSprites.Source as BitmapSource,
                new Int32Rect(charPos * 48, 0 * 48, 48, 48));
            rightChar.Source = new CroppedBitmap(rightCharSprites.Source as BitmapSource,
                new Int32Rect(charPos * 48, 0 * 48, 48, 48));
            charPos = charPos >= 2 ? 0 : charPos + 1;
        }

        private void CreateTimer(Action<object, EventArgs> action, int milliseconds)
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(action);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds);
            dispatcherTimer.Start();
        }
    }
}
