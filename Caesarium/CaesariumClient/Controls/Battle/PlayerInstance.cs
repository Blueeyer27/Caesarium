using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CaesariumClient.Controls.Battle
{
    public class PlayerInstance
    {
        public int x, y;
        public int skinNumber = 0;
        public Image Sprite { get; set; }
        public Image Lightning { get; set; }
        public Image IceBarrier { get; set; }

        public PlayerInstance(Image sprite, int x = 0, int y = 0)
        {
            skinNumber = (new Random(DateTime.Now.Millisecond)).Next(0, 8);
            this.x = x;
            this.y = y;
            Sprite = sprite;
            Lightning = new Image();
            Lightning.Stretch = Stretch.Fill;
        }

        public void LightningHit()
        {

        }

        public void AnimateMove(int x, int y)
        {
            //Sprite.Source = new CroppedBitmap(Sprite.Source as BitmapSource, new Int32Rect(275, 0, 130, 90));

            this.x = x;
            this.y = y;
        }
    }
}
