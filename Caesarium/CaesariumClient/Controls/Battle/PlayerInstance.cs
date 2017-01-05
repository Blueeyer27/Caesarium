using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CaesariumClient.Controls.Battle
{
    public class PlayerInstance
    {
        public int x, y;
        public Image Sprite { get; set; }
        public Image Lightning { get; set; }

        public PlayerInstance(Image sprite, int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
            Sprite = sprite;
        }
    }
}
