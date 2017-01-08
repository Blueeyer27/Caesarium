using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesariumServer.Battle
{
    public class Skill
    {
        public int hitX;
        public int hitY;

        public string name;

        public Coords direction;

        public int range;

        public Skill (string name, int hitX, int hitY, int range, Coords direction = null)
        {
            this.name = name;

            this.hitX = hitX;
            this.hitY = hitY;

            this.range = range;

            this.direction = direction == null ? new Coords(0, 0) : direction;
        }
    }
}
