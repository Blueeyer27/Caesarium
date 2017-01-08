using CaesariumServer.Battle;
//using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaesariumServer
{
    public class Coords
    {
        public int x;
        public int y;

        public Coords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Program
    {
        const int port = 6112;
        static TcpListener listener;
        static List<Game> games = new List<Game>();


        static void Main(string[] args)
        {
            // CaesariumDatabase.Create();
            //string connStr = "server=localhost;user=root;port=3306;password=qwerty11;";
            //using (var conn = new MySqlConnection(connStr))
            //using (var cmd = conn.CreateCommand())
            //{
            //    conn.Open();
            //    cmd.CommandText = "CREATE DATABASE IF NOT EXISTS `hello`;";
            //    cmd.ExecuteNonQuery();
            //}

            try
            {
                //listener = new TcpListener(IPAddress.Parse("87.110.165.82"), port);
                listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
                listener.Start();
                Console.WriteLine("Waiting for connections...");

                games.Add(new Game(new BattleField(), 4));

                int id = 0;
                while (true)
                {
                    GameClient client = new GameClient(listener.AcceptTcpClient(), id);
                    id++;

                    if (games[games.Count - 1].Clients.Count < games[games.Count - 1].MaxPlayers)
                    {
                        games[games.Count - 1].Clients.Add(client);
                        client.SetCurrentGame(games[games.Count - 1]);
                    }
                    else
                    {
                        BattleField newField = new BattleField();
                        var game = new Game(newField);

                        //TODO: let Game do this itself
                        client.SetCurrentGame(game);
                        game.Clients.Add(client);


                        games.Add(game);
                    }

                    client.InitPlayers();
                    client.ClientThread = new Thread(new ThreadStart(client.Process));

                    client.ClientThread.Start();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
    }

    public class Game
    {
        public int MaxPlayers { get; set; } //TODO:
        public BattleField gameField;
        public List<GameClient> Clients = new List<GameClient>();
        public Game(BattleField field, int maxPlayers = 2)
        {
            MaxPlayers = maxPlayers;
            gameField = field;
        }

        //TODO: Move this function to battlefield
        public void HitOpponents(GameClient sender, List<Coords> hitCoords, int damage)
        {
            foreach (var client in Clients)
            {
                if (client != sender)
                    foreach (var opponent in client.Players)
                    {
                        foreach (var coord in hitCoords)
                        {
                            if (coord.x <= opponent.X + 27 && coord.x >= opponent.X - 27
                                && coord.y <= opponent.Y + 27 && coord.y >= opponent.Y - 27)
                            {
                                opponent.Hp -= damage;
                                if (opponent.Hp <= 0)
                                    opponent.Dead = true;
                                Console.WriteLine(opponent.Name + "  HP: " + opponent.Hp + "/60  coords: X = " + opponent.X + " Y = " + opponent.Y
                                    + "\nAttacker coords: X = " + sender.Players[0].X + " Y = " + sender.Players[0].Y);
                                break;
                            }
                        }
                    }
            }
        }

        internal void SkillUse(string skill, GameClient sender, PlayerInstance attacker)
        {
            switch (skill)
            {
                case "light":
                    if (!attacker.CanLightningHit()) return;

                    foreach (var client in Clients)
                    {
                        client.unhandledSkills.Add(new Skill(skill, attacker.X, attacker.Y, attacker.lightRange, attacker.GetDirection()));
                        HitOpponents(sender, attacker.LightningHit(), attacker.lightDmg);
                    }
                    break;
                case "barr":
                    if (!attacker.CanBarrierHit()) return;

                    foreach (var client in Clients)
                    {
                        client.unhandledSkills.Add(new Skill(skill, attacker.X, attacker.Y, attacker.barrRange));
                        HitOpponents(sender, attacker.BarrierHit(), attacker.barrDmg);
                    }
                    break;
            }
        }
    }

    public class GameClient
    {
        public Thread ClientThread { get; set; }
        public List<PlayerInstance> Players;

        public List<Skill> unhandledSkills;

        Game currGame;

        public TcpClient client;
        public int id;

        public GameClient(TcpClient client, int id)
        {
            this.id = id;
            this.client = client;

            Players = new List<PlayerInstance>();
            unhandledSkills = new List<Skill>();
        }

        public void SetCurrentGame(Game currGame)
        {
            this.currGame = currGame;
        }

        public void InitPlayers()
        {
            if (Players.Count == 0)
            {
                Players.Add(new PlayerInstance("Antowa" + id, new char[] { 'W', 'A', 'S', 'D' }, new char[] { 'C', 'V' }));
                Players.Add(new PlayerInstance("Anton" + id, new char[] { 'I', 'J', 'K', 'L' }, new char[] { 'N', 'B' }));
            }
        }

        public void Process()
        {
            DateTime lastReq = DateTime.Now;

            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                StringBuilder builder = new StringBuilder();

                var counter = 0;
                while (true)
                {
                    counter++;
                    // Getting message
                    builder.Clear();
                    byte[] data = new byte[64]; // data buffer;
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);

                    string message = builder.ToString();
                    var commands = message.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    var funcWithArgs = commands[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    var func = funcWithArgs[0];
                    var args = commands[0].Substring(func.Length + 1);

                    //string response = x + ":" + y;

                    //Console.WriteLine("\n Function: " + func + " Arguments: " + args);
                    //Console.WriteLine(response);
                    // sending response

                    var responseSb = new StringBuilder(" ");

                    if (func == "action")
                    {
                        TimeSpan span = DateTime.Now - lastReq;
                        if (span.TotalMilliseconds < 35)
                        {
                            data = Encoding.Unicode.GetBytes(" ");
                            stream.Write(data, 0, data.Length);
                            continue;
                        }

                        //Console.WriteLine(id + "  " + counter);
                        lastReq = DateTime.Now;
                        MakeMove(args);

                        data = Encoding.Unicode.GetBytes(responseSb.ToString());
                        stream.Write(data, 0, data.Length);
                    }
                    else if (func == "getObj")
                    {
                        foreach (var gameClient in currGame.Clients)
                            foreach (var player in gameClient.Players)
                                if (player.Dead) responseSb.Append("-1:-1/");
                                else responseSb.Append(player.X + ":" + player.Y + "/");

                        if (unhandledSkills.Count > 0)
                        {
                            foreach (var skill in unhandledSkills)
                            {
                                responseSb.Append("#" + skill.name + ":" + skill.hitX + ":" + skill.hitY 
                                    + ":" + skill.direction.x + ":" + skill.direction.y + ":" + skill.range + "#");
                            }

                            unhandledSkills.Clear();
                        }

                        var resp = responseSb.ToString();

                        data = Encoding.Unicode.GetBytes(resp);
                        stream.Write(data, 0, data.Length);
                    }
                    else
                    {
                        data = Encoding.Unicode.GetBytes(responseSb.ToString());
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

        private void MakeMove(string args)
        {
            foreach (var player in Players)
            {
                var prevMove = new Coords(player.X, player.Y);

                foreach (var ch in args)
                {
                    if (ch == player.moveButtons[0])
                    {
                        player.Y -= player.step;
                        player.madeMove = true;
                    }
                    else if (ch == player.moveButtons[1])
                    {
                        player.X -= player.step;
                        player.madeMove = true;
                    }
                    else if (ch == player.moveButtons[2])
                    {
                        player.Y += player.step;
                        player.madeMove = true;
                    }
                    else if (ch == player.moveButtons[3])
                    {
                        player.X += player.step;
                        player.madeMove = true;
                    }
                }

                if (player.madeMove)
                    player.SavePrevMove(prevMove);

                foreach (var ch in args)
                {
                    if (!player.Dead)
                        if (ch == player.skillButtons[0])
                            currGame.SkillUse("light", this, player);
                        else if (ch == player.skillButtons[1])
                            currGame.SkillUse("barr", this, player);
                }
            }
        }
    }
}
