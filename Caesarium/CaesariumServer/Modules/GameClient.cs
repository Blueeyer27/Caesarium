using CaesariumServer.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaesariumServer.Modules
{
    public class GameClient
    {
        public Thread ClientThread { get; set; }
        public List<PlayerInstance> Players;

        public List<Skill> unhandledSkills;

        Game currGame;

        public TcpClient client;
        public Coords StartCoords { get; set; }
        public int id;

        public GameClient(TcpClient client)
        {
            this.id = 0;
            this.client = client;
            StartCoords = new Coords(0, 0);

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
                Players.Add(new PlayerInstance("Player1_" + id, this, new BattleControls('W', 'S', 'A', 'D', 'C', 'V'), StartCoords.x, StartCoords.y));
                Players.Add(new PlayerInstance("Player2_" + id, this, new BattleControls('I', 'K', 'J', 'L', 'N', 'B'), StartCoords.x, StartCoords.y));
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

                    var responseSb = new StringBuilder("");

                    if (id != 0)
                    {
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
                        }
                        else if (func == "startGame")
                        {
                            StartBattle();
                        }
                    }
                    else
                    {
                        if (func == "log")
                        {
                            var logPwd = args.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                            responseSb.Append(Login(logPwd[0], logPwd[1]));
                        }
                    }

                    if (responseSb.Length < 1)
                        responseSb.Append(" ");

                    data = Encoding.Unicode.GetBytes(responseSb.ToString());
                    stream.Write(data, 0, data.Length);
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

        private string Login(string login, string password) 
        {
            var answer = CaesariumDatabase.ExecuteQuery("SELECT * FROM accounts WHERE login = @login", "login", login);

            if (answer.Count == 0)
            {
                var id = CaesariumDatabase.ExecuteQuery(
                    "INSERT INTO accounts (Login, Password) " + 
                    "VALUES (@login, @password); " + 
                    "SELECT last_insert_id();", "login", login, "password", password);

                return id[0];
            }
            else
            {
                var id = answer[0];
                this.id = int.Parse(id);

                var dbPassword = answer[2];

                return dbPassword.CompareTo(password) == 0 ? id : "0";
            }
        }

        private void StartBattle()
        {
            var games = Program.games;
            var lastGame = games[games.Count - 1];
            if (lastGame.Clients.Count < lastGame.MaxClients)
            {
                if (lastGame.Clients.Count > 0)
                    StartCoords = new Coords(840, 480);

                lastGame.Clients.Add(this);
                SetCurrentGame(lastGame);
            }
            else
            {
                BattleField newField = new BattleField();
                var game = new Game(newField);

                //TODO: let Game do this itself
                SetCurrentGame(game);
                game.Clients.Add(this);


                Program.games.Add(game);
            }

            InitPlayers();

            if (lastGame.Clients.Count == lastGame.MaxClients)
                lastGame.started = true;
        }

        private void MakeMove(string args)
        {
            if (!currGame.started) return;

            foreach (var player in Players)
            {
                var prevMove = new Coords(player.X, player.Y);

                foreach (var ch in args)
                {
                    if (ch == player.Controls.Up)
                    {
                        player.Y -= player.step;
                        player.madeMove = true;
                    }
                    else if (ch == player.Controls.Left)
                    {
                        player.X -= player.step;
                        player.madeMove = true;
                    }
                    else if (ch == player.Controls.Down)
                    {
                        player.Y += player.step;
                        player.madeMove = true;
                    }
                    else if (ch == player.Controls.Right)
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
                        if (ch == player.Controls.MainSkill)
                            currGame.SkillUse("light", player);
                        else if (ch == player.Controls.MassiveSkill)
                            currGame.SkillUse("barr", player);
                }
            }
        }
    }
}
