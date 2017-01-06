using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesariumServer
{
    public class ObjectInstance
    {
        public Coords prevMove = new Coords(0, 0);
        public string Name { get; set; }
        protected int height = 30, width = 30;

        public int Hp { get; set; }

        private int _x;
        public int X 
        { 
            get { return _x; }
            set { if (value >= 0) { _x = value; } } 
        }

        private int _y;
        public int Y
        {
            get { return _y; }
            set { if (value >= 0) { _y = value; } }
        }

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
