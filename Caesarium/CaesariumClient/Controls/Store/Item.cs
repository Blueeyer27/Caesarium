using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CaesariumClient.Controls.Store
{
    public class Item
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public int Power { get; set; }
        public int Defence { get; set; }
        public int Hp { get; set; }

        public Label ImageLabel { get; set; }

        public Item(long id, string name, string imageName, int power = 0, int defence = 0, int hp = 0)
        {
            string appPath = Directory.GetCurrentDirectory();
            appPath = appPath.Substring(0, appPath.Length - 10);

            ID = id;
            Name = name;

            ImageLabel = new Label()
            {
                Background = new ImageBrush(new BitmapImage(
                    new Uri(appPath + "/Images/Items/" + imageName)))
            };

            Power = power;
            Defence = defence;
            Hp = hp;
        }
    }
}
