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
    public class Skill
    {
        public Image AllSpriteStates { get; set; }
        public Image Sprite { get; set; }
        public int cropStep, spriteHeight;
        public int aliveTime;
        public DateTime created;

        private int _stateCount, _handledStates = 1;

        public Skill(Image sprite, int spriteHeight, int cropStep, int stateCount = 1, int aliveTime = -1)
        {
            AllSpriteStates = sprite;
            Sprite = new Image();
            Sprite.Source = new CroppedBitmap(AllSpriteStates.Source as BitmapSource,
                new Int32Rect(0, 0, cropStep, spriteHeight));
            Sprite.Stretch = Stretch.Fill;

            this.spriteHeight = spriteHeight;
            this.cropStep = cropStep;

            this.aliveTime = aliveTime;
            _stateCount = stateCount;

            created = DateTime.Now;
        }

        public void SetAngle(double angle, int centerX = 0, int centerY = 0) {
            Sprite.RenderTransform = new RotateTransform(angle, centerX, centerY);
        }

        public bool CanRemoveAnimation()
        {
            if (aliveTime > 0)
            {
                TimeSpan diff = DateTime.Now - created;
                return !(diff.Milliseconds < aliveTime);
            }
            else return _handledStates >= _stateCount;
        }

        public void Animate()
        {
            if (_handledStates >= _stateCount)
                if (aliveTime > 0) _handledStates = 0;
                else return;

            Sprite.Source = new CroppedBitmap(AllSpriteStates.Source as BitmapSource, 
                new Int32Rect(cropStep * _handledStates, 0, cropStep, spriteHeight));
            _handledStates++;
        }
    }
}
