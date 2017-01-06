using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace CaesariumClient.Controls.Battle
{
    public class PlayerInstance
    {
        public int x, y;
        public Image Sprite { get; set; }
        public Image Lightning { get; set; }
        public Image IceBarrier { get; set; }

        public PlayerInstance(Image sprite, int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
            Sprite = sprite;
            Lightning = new Image();
            Lightning.Stretch = Stretch.Fill;
        }

        public void LightningHit()
        {

        }
    }
}
