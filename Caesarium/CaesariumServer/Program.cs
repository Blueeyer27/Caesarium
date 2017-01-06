using MySql.Data.MySqlClient;
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
            CaesariumDatabase.Create();
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
        public void HitOpponents(GameClient sender, List<Coords> hitCoords)
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
                                opponent.Hp -= 15;
                                Console.WriteLine(opponent.Name + "  HP: " + opponent.Hp + "/60  coords: X = " + opponent.X + " Y = " + opponent.Y 
                                    + "\nAttacker coords: X = " + sender.Players[0].X + " Y = " + sender.Players[0].Y);
                                break;
                            }
                        }
                    }
            }
        }
    }

    public class GameClient
    {
        public Thread ClientThread { get; set; }
        public List<PlayerInstance> Players;
        Game currGame;

        public TcpClient client;
        public int id;

        public GameClient(TcpClient client, int id)
        {
            this.id = id;
            this.client = client;
            

            Players = new List<PlayerInstance>();
        }

        public void SetCurrentGame(Game currGame)
        {
            this.currGame = currGame;
        }

        public void InitPlayers()
        {
            if (Players.Count == 0)
            {
                Players.Add(new PlayerInstance("Antowa" + id));
                Players.Add(new PlayerInstance("Anton" + id));
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

                    if (func == "action")
                    {
                        TimeSpan span = DateTime.Now - lastReq;
                        if (span.TotalMilliseconds < 10)
                        {
                            data = Encoding.Unicode.GetBytes(" ");
                            stream.Write(data, 0, data.Length);
                            continue;
                        }

                        //Console.WriteLine(id + "  " + counter);
                        lastReq = DateTime.Now;

                        MakeMove(args);

                        data = Encoding.Unicode.GetBytes(" ");
                        stream.Write(data, 0, data.Length);
                    }
                    else if (func == "getObj")
                    {
                        //TODO: REMOVE THIS!!!!!!! 
                        foreach (var gameClient in currGame.Clients)
                            foreach (var player in gameClient.Players)
                                responseSb.Append(player.X + ":" + player.Y + "/");

                        var resp = responseSb.ToString();

                        data = Encoding.Unicode.GetBytes(resp);
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
            var prevMove0 = new Coords(Players[0].X, Players[0].Y);
            var prevMove1 = new Coords(Players[1].X, Players[1].Y);

            Console.WriteLine(prevMove0.x + " " + prevMove0.y);
            foreach (var ch in args)
            {
                //bool delete = true;
                switch (ch)
                {
                    //Moves
                    case 'W':
                        Players[0].Y -= Players[0].step;
                        Players[0].madeMove = true;
                        break;
                    case 'I':
                        Players[1].Y -= Players[1].step;
                        Players[1].madeMove = true;
                        break;
                    case 'A':
                        Players[0].X -= Players[0].step;
                        Players[0].madeMove = true;
                        break;
                    case 'J':
                        Players[1].X -= Players[1].step;
                        Players[1].madeMove = true;
                        break;
                    case 'S':
                        Players[0].Y += Players[0].step;
                        Players[0].madeMove = true;
                        break;
                    case 'K':
                        Players[1].Y += Players[1].step;
                        Players[1].madeMove = true;
                        break;
                    case 'D':
                        Players[0].X += Players[0].step;
                        Players[0].madeMove = true;
                        break;
                    case 'L':
                        Players[1].X += Players[1].step;
                        Players[1].madeMove = true;
                        break;
                    //default:
                    //    delete = false;
                    //    break;
                }

                //if (delete)
                //{
                //    String.Join("", args.Split(ch));
                //}
            }

            if (Players[0].madeMove)
                Players[0].SavePrevMove(prevMove0);
            if (Players[1].madeMove)
                Players[1].SavePrevMove(prevMove1);

            foreach (var ch in args)
            {
                switch (ch)
                {
                    //Skills
                    case 'Q':
                        currGame.HitOpponents(this, Players[0].LightningHit());
                        break;
                    case 'E':
                        break;
                    case 'U':
                        break;
                    case 'O':
                        break;
                }
            }
        }
    }
}
