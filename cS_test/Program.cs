using Socket.Messages;
using Socket.Client;
using Socket.Server;

using System.Net;

namespace cS_test
{

    public static class Program {

        public static void Main(string[] args) {

            SocketS s = new SocketS(IPEndPoint.Parse("127.0.0.1:1236"));
            Console.WriteLine($"server rodando");
            s.OnClientAccepted += S_OnClientAccepted;
            s.OnMessageReceived += S_OnMessageReceived;
            s.OnClientDisconnect += S_OnClientDisconnect;
            s.OnClientChangeChannel += S_OnClientChangeChannel;
            s.Connect();

            Console.ReadKey();

            Console.WriteLine("dsadsadsa");
        
        }

        private static void S_OnClientChangeChannel(Socket.Messages.Body.ChangeChannelBody arg1, SocketC arg2)
        {
            Console.WriteLine($"cliente {arg2.UserName} foi de {arg1.From} para {arg1.To}");
        }

        private static void S_OnClientDisconnect(SocketC obj)
        {
            Console.WriteLine("cliente saiu : " +  obj.UserName);
        }

        private static void S_OnMessageReceived(Message arg1, SocketC arg2)
        {
            Console.WriteLine("message : " + arg1);
        }

        private static void S_OnClientAccepted(SocketC obj)
        {
            Console.WriteLine($"novo cliente : " + obj.UserName);
        }
    }

}
