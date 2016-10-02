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
    public class ClientObject
    {
        int x = 5;
        int y = 5;
        public TcpClient client;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
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
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    string response = "Simple response";
                    if (message.Contains("Query:Left"))
                        if (x > 0)
                        {
                            x--;
                            response = "You moved left" + ": x =" + x + " y=" + y;
                        }
                        else
                            response = "You are too far left" + ": x =" + x + " y=" + y;
                    if (message.Contains("Query:Right"))
                        if (x < 10)
                        {
                            x++;
                            response = "You moved right" + ": x =" + x + " y=" + y;
                        }
                        else
                            response = "You are too far right" + ": x =" + x + " y=" + y;
                    if (message.Contains("Query:Down"))
                        if (y > 0)
                        {
                            y--;
                            response = "You moved down" + ": x =" + x + " y=" + y;
                        }
                        else
                            response = "You are at bottom" + ": x =" + x + " y=" + y;
                    if (message.Contains("Query:Top"))
                        if (x < 10)
                        {
                            y++;
                            response = "You moved to top" + ": x =" + x + " y=" + y;
                        }
                        else
                            response = "You are too far at top" + ": x =" + x + " y=" + y;


                    Console.WriteLine(message);
                    Console.WriteLine(response);
                    // sending response

                    data = Encoding.Unicode.GetBytes(response);
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
    }
    class Program
    {

        const int port = 8888;
        static TcpListener listener;
        static void Main(string[] args)
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                Console.WriteLine("Waiting for connections...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);

                    // New thread for new client
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
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
}
