using MySocket.Messages;
using MySocket.Client;
using MySocket.Server;

using System.Net;

namespace cS_test
{

    public static class Program {

        public static void Main(string[] args) {


            Console.Title = "Server";

            SocketS server = new SocketS(IPEndPoint.Parse("127.0.0.1:1236"));
            
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

        private static void S_OnTryUpServerFail(SocketS obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Erro on trying up server");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnServerDown(SocketS obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Server offline");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnServerUp(SocketS obj)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Server online");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnClientChangeChannel(MySocket.Messages.Body.ChangeChannelBody arg1, SocketC arg2)
        {
            Console.WriteLine($"Client {arg2.UserName} change {arg1.From} to {arg1.To}");
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnClientDisconnect(SocketC obj)
        {
            Console.WriteLine("Client disconnected: " +  obj.UserName);
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnMessageReceived(Message arg1, SocketC arg2)
        {
            Console.WriteLine("Message: " + arg1);
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }

        private static void S_OnClientAccepted(SocketC obj)
        {
            Console.WriteLine($"Client connected: " + obj.UserName);
            Console.WriteLine();
            Console.WriteLine("Type a command:");
        }
    }

}
