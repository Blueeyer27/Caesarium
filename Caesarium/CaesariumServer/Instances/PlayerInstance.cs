using CaesariumServer.Battle;
using CaesariumServer.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesariumServer
{
    public class PlayerInstance : ObjectInstance
    {
        public int step = 8;

        public int lightRange = 300, lightCountdown = 500;
        public int lightDmg = 3;
        private DateTime lastLightCast = DateTime.Now.AddHours(-1);

        public BattleControls Controls { get; set; }
        public GameClient Client { get; set; }

        public int barrRange = 100, barrCountdown = 3000;
        public int barrDmg = 2;
        private DateTime lastBarrierCast = DateTime.Now.AddHours(-1);

        public bool madeMove = false;

        public int Power { get; set; }

        public PlayerInstance(string name, GameClient client, BattleControls controls, int x = 0, int y = 0, int hp = 60, int power = 5)
            : base(name, x, y, hp)
        {
            Client = client;
            Controls = controls;
            Power = power;
        }

        public Coords GetDirection()
        {
            return new Coords(X - prevMove.x, Y - prevMove.y);
        }

        public void SavePrevMove(Coords move)
        {
            prevMove.x = move.x;
            prevMove.y = move.y;
            madeMove = false;
        }

        public bool CanLightningHit()
        {
            TimeSpan diff = DateTime.Now - lastLightCast;
            return diff.TotalMilliseconds >= lightCountdown;
        }

        public List<Coords> LightningHit()
        {
            var direction = GetDirection();

            List<Coords> hitCoords = new List<Coords>();

            if (CanLightningHit())
            {
                for (var i = 0; i < lightRange / step; i++)
                {
                    int xRange = direction.x * i;
                    int yRange = direction.y * i;
                    if (direction.x != 0 && direction.y != 0)
                    {
                        hitCoords.Add(new Coords(X + (int)(xRange * 0.707), Y + (int)(yRange * 0.707)));
                    }
                    else hitCoords.Add(new Coords(X + xRange, Y + yRange));
                }

                lastLightCast = DateTime.Now;
            }

            return hitCoords;
        }

        public bool CanBarrierHit()
        {
            TimeSpan diff = DateTime.Now - lastBarrierCast;
            return diff.TotalMilliseconds >= barrCountdown;
        }

        public List<Coords> BarrierHit()
        {
            List<Coords> hitCoords = new List<Coords>();


            if (CanBarrierHit())
            {
                int startX = X - barrRange/2;
                int startY = Y - barrRange/2;

                for (var x = 0; x < barrRange; x++)
                {
                    for (var y = 0; y < barrRange; y++)
                    {
                        hitCoords.Add(new Coords(startX + x, startY + y));
                    }
                }

                lastBarrierCast = DateTime.Now;
            }

            return hitCoords;
        }
    }
}
