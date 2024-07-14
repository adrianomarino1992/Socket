using MySocket.Messages;
using MySocket.Client;
using MySocket.Server;

using System.Net;

namespace cS_test
{

    public static class Program {

        public static void Main(string[] args) {


            Console.Title = "Server";

            SocketServer server = new SocketServer(IPEndPoint.Parse("192.168.15.93:1236"));
            
            server.OnClientAccepted += S_OnClientAccepted;
            server.OnMessageReceived += S_OnMessageReceived;
            server.OnClientDisconnect += S_OnClientDisconnect;
            server.OnClientChangeChannel += S_OnClientChangeChannel;
            server.OnServerUp += S_OnServerUp;
            server.OnServerDown += S_OnServerDown;
            server.OnTryUpServerFail += S_OnTryUpServerFail;
            server.Connect();            

            while(true)
            {
                string l = Console.ReadLine();

                if (l == "-o")
                {
                    server.Connect();
                }

                if(l == "-h")
                {
                    server.Disconnect();
                }

            }            
        
        }

        private static void S_OnTryUpServerFail(SocketServer obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Erro on trying up server");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnServerDown(SocketServer obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Server offline");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnServerUp(SocketServer obj)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Server online");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnClientChangeChannel(MySocket.Messages.Body.ChangeChannelBody arg1, SocketClient arg2)
        {
            Console.WriteLine($"Client {arg2.UserName} change {arg1.From} to {arg1.To}");
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnClientDisconnect(SocketClient obj)
        {
            Console.WriteLine("Client disconnected: " +  obj.UserName);
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnMessageReceived(Message arg1, SocketClient arg2)
        {
            Console.WriteLine("Message: " + arg1);
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnClientAccepted(SocketClient obj)
        {
            Console.WriteLine($"Client connected: " + obj.UserName);
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }
    }

}
