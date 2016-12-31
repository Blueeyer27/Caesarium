using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesariumServer
{
    class PlayerInstance : ObjectInstance
    {
        public int Power { get; set; }

        public PlayerInstance(string name) : base(name)
        {
            Power = 10;
        }

        public PlayerInstance(string name, int x, int y, int hp = 60, int power = 10) : base(name, x, y, hp)
        {
            Power = power;
        }

        public void Move(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
