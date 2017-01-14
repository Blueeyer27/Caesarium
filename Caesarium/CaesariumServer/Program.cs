using CaesariumServer.Battle;
using CaesariumServer.Modules;
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
        public static List<Game> games = new List<Game>();


        static void Main(string[] args)
        {
            CaesariumDatabase.Create();

            try
            {
                //listener = new TcpListener(IPAddress.Parse("87.110.165.82"), port);
                listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
                listener.Start();
                Console.WriteLine("Waiting for connections...");

                games.Add(new Game(new BattleField(), 2));
                
                while (true)
                {
                    GameClient client = new GameClient(listener.AcceptTcpClient());
                    
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
}
