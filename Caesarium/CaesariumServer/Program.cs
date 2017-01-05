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
    class Program
    {
        const int port = 20012;
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
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                Console.WriteLine("Waiting for connections...");

                games.Add(new Game(new BattleField(), 4));
                
                while (true)
                {
                    GameClient client = new GameClient(listener.AcceptTcpClient());

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
    }

    public class GameClient
    {
        public Thread ClientThread { get; set; }
        List<PlayerInstance> Players;
        Game currGame;

        public TcpClient client;

        public GameClient(TcpClient client)
        {
            this.client = client;

            Players = new List<PlayerInstance>();
        }

        public void SetCurrentGame(Game currGame)
        {
            this.currGame = currGame;
        }

        public void InitPlayers() {
            if (Players.Count == 0){
                Players.Add(new PlayerInstance("Anton"));
                Players.Add(new PlayerInstance("Anton"));
            }
        }

        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64]; // data buffer;
                while (true)
                {
                    // Getting message
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);

                    string message = builder.ToString();
                    var funcWithArgs = message.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    var func = funcWithArgs[0];
                    var args = message.Substring(func.Length + 1);

                    //string response = x + ":" + y;

                    //Console.WriteLine("\n Function: " + func + " Arguments: " + args);
                    //Console.WriteLine(response);
                    // sending response

                    var responseSb = new StringBuilder("");

                    if (func == "move")
                    {
                        MakeMove(args);
                    }
                    else if (func == "getObj")
                    {
                        //TODO: REMOVE THIS!!!!!!! 
                        var firstClient = currGame.Clients[0].Players;
                        foreach (var player in firstClient)
                        {
                            responseSb.Append(player.X + ":" + player.Y + "/");
                        }

                        //Console.WriteLine(response);
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
            var step = 5;

            foreach (var ch in args)
            {
                switch (ch)
                {
                    case 'W':
                        Players[0].Y -= step;
                        break;
                    case 'I':
                        Players[1].Y -= step;
                        break;
                    case 'A':
                        Players[0].X -= step;
                        break;
                    case 'J':
                        Players[1].X -= step;
                        break;
                    case 'S':
                        Players[0].Y += step;
                        break;
                    case 'K':
                        Players[1].Y += step;
                        break;
                    case 'D':
                        Players[0].X += step;
                        break;
                    case 'L':
                        Players[1].X += step;
                        break;
                }
            }

        }
    }
}
