using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CaesariumClient.Controls
{
    public partial class BattleControl : UserControl
    {
        Grid objectFieldGrid = new Grid();

        private void AddBattleObject(int x, int y, Image obj) {
            Grid.SetColumn(obj, 0);
            Grid.SetRow(obj, 0);
            Label label = new Label();
            label.Content = "-10";
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, 0);
            var margin = obj.Margin;
            margin.Left = x;
            margin.Top = y;
            obj.Margin = margin;

            objectFieldGrid.Children.Add(obj);
            objectFieldGrid.Children.Add(label);
        }

        private void MoveBattleObject(int x, int y, Image obj)
        {
            //Grid.SetColumn(obj, x);
            //Grid.SetRow(obj, y);
            
            var margin = obj.Margin;
            margin.Left = x;
            margin.Top = y;
            obj.Margin = margin;
        }

        private void RemoveBattleObject(Image obj) {
            objectFieldGrid.Children.Remove(obj);
        }

        private void InitializeBattleObjects()
        {
            
            //objectFieldGrid.
            objectsField = new UserControl();
            objectsField.Content = objectFieldGrid;

            for (var i = 0; i < 1; i++)
            {
                var colDef = new ColumnDefinition();
                colDef.Width = new GridLength(1, GridUnitType.Star);
                objectFieldGrid.ColumnDefinitions.Add(colDef);
            }

            for (var i = 0; i >= 0; i--)
            {
                var rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                objectFieldGrid.RowDefinitions.Add(rowDef);
            }

            contentControl.Content = objectsField;
            objectFieldGrid.ClipToBounds = true;
        }
    }
}
