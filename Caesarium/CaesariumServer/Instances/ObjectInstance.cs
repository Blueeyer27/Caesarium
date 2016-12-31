using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesariumServer
{
    public class ObjectInstance
    {
        public string Name { get; set; }
        protected int height = 30, width = 30;

        public int Hp { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public ObjectInstance(string name)
        {
            Name = name;
            Hp = 60;
            X = 0;
            Y = 0;
        }

        public ObjectInstance(string name, int x, int y, int hp = 60)
        {
            Name = name;
            X = x;
            Y = y;
            Hp = hp;
        }
    }
}
