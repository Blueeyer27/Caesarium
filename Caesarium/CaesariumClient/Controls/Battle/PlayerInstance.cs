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
        public Image AllSpriteStates { get; set; }
        public Image DeadSprite { get; set; }
        public Image Sprite { get; set; }

        private DateTime lastReq;
        private static int DOWN = 0, RIGHT = 1, LEFT = 2, UP = 3;

        private bool _dead;
        public bool Dead { 
            get { return _dead; }
            set
            {
                if (!_dead)
                {
                    _dead = true;
                    if (true)
                    {
                        Sprite.Source = new CroppedBitmap(DeadSprite.Source as BitmapSource, new Int32Rect(0, 0, 100, 100));
                    }
                }
            }
        }

        int xOffset = 0, yOffset = 0;

        public PlayerInstance(Image sprite, int x = 0, int y = 0)
        {
            skinNumber = (new Random(DateTime.Now.Millisecond)).Next(0, 8);
            this.x = x;
            this.y = y;
            Sprite = sprite;

            lastReq = DateTime.Now;
        }

        public void AnimateMove(int x, int y)
        {
            if (Dead) { Sprite.Source = DeadSprite.Source; return; }

            var xDiff = x - this.x;
            var yDiff = y - this.y;

            this.x = x;
            this.y = y;

            if (yDiff < 0)
            {
                yOffset = UP;
            }
            else if (yDiff > 0)
            {
                yOffset = DOWN;
            }
            else if (xDiff < 0)
            {
                yOffset = RIGHT;
            }
            else if (xDiff > 0)
            {
                yOffset = LEFT;
            }

            if (xDiff == 0 && yDiff == 0)
            {
                xOffset = 1;
            }
            else
            {
                xOffset = xOffset >= 2 ? 0 : xOffset + 1;
            }

            Sprite.Source = new CroppedBitmap(AllSpriteStates.Source as BitmapSource,
                new Int32Rect(xOffset * 48, yOffset * 48, 48, 48));
        }
    }
}
