using System;
using System.Threading;
using ByteNom;

namespace MultiplayerNom.Demo
{
    internal class Program
    {
        private static void Main()
        {
            // Start a server
            var server = new Server(12345);
            var mutliplayerServer = new MultiplayerServer<Lobby<ServerRoom>>(server);
            server.Start();

            // Start a client
            var client = new Client("localhost", 12345);
            client.MessageReceived += client_MessageReceived;
            client.Connect();
            // Join a room
            client.Send("join", "testroom");
            // Send a message
            client.Send("whoami");

            Thread.Sleep(Timeout.Infinite);
        }

        // Called whenever our client receives a message
        private static void client_MessageReceived(object sender, Message message)
        {
            if (message.Type == "join")
            {
                Console.WriteLine("{0} joined the room.", message.GetString(0));
            }
            else if (message.Type == "leave")
            {
                Console.WriteLine("{0} left the room.", message.GetString(0));
            }
            else if (message.Type == "name")
            {
                Console.WriteLine("Name was set to {0}.", message.GetString(0));
            }
        }
    }
}