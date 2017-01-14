using CaesariumServer.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesariumServer.Modules
{
    public class Game
    {
        public int MaxClients { get; set; } //TODO:
        public BattleField gameField;
        public List<GameClient> Clients = new List<GameClient>();

        public bool started = false;

        public Game(BattleField field, int maxPlayers = 2)
        {
            MaxClients = maxPlayers;
            gameField = field;
        }

        //TODO: Move this function to battlefield
        public void HitOpponents(PlayerInstance attacker, List<Coords> hitCoords, int damage)
        {
            foreach (var client in Clients)
            {
                if (client != attacker.Client)
                    foreach (var opponent in client.Players)
                    {
                        foreach (var coord in hitCoords)
                        {
                            if (coord.x <= opponent.X + 27 && coord.x >= opponent.X - 27
                                && coord.y <= opponent.Y + 27 && coord.y >= opponent.Y - 27)
                            {
                                opponent.Hp -= damage * attacker.Power;
                                if (opponent.Hp <= 0)
                                    opponent.Dead = true;
                                Console.WriteLine(opponent.Name + "  HP: " + opponent.Hp + "/60  coords: X = " + opponent.X + " Y = " + opponent.Y
                                    + "\nAttacker coords: X = " + attacker.X + " Y = " + attacker.Y);
                                break;
                            }
                        }
                    }
            }
        }

        internal void SkillUse(string skill, PlayerInstance attacker)
        {
            switch (skill)
            {
                case "light":
                    if (!attacker.CanLightningHit()) return;

                    foreach (var client in Clients)
                    {
                        client.unhandledSkills.Add(new Skill(skill, attacker.X, attacker.Y, attacker.lightRange, attacker.GetDirection()));
                        HitOpponents(attacker, attacker.LightningHit(), attacker.lightDmg);
                    }
                    break;
                case "barr":
                    if (!attacker.CanBarrierHit()) return;

                    foreach (var client in Clients)
                    {
                        client.unhandledSkills.Add(new Skill(skill, attacker.X, attacker.Y, attacker.barrRange));
                        HitOpponents(attacker, attacker.BarrierHit(), attacker.barrDmg);
                    }
                    break;
            }
        }
    }
}
