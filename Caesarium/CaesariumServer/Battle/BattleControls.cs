using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesariumServer.Battle
{
    public class BattleControls
    {
        public char Up { get; set; }
        public char Down { get; set; }
        public char Left { get; set; }
        public char Right { get; set; }

        public char MainSkill { get; set; }
        public char MassiveSkill { get; set; }

        public BattleControls(char up, char down, char left, char right, char mainSkill, char massiveSkill)
        {
            Up = up;
            Down = down;
            Left = left;
            Right = right;

            MainSkill = mainSkill;
            MassiveSkill = massiveSkill;
        }
    }
}
