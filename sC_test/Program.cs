using Socket.Messages;
using Socket.Client;
using Socket.Server;

using System.Net;

namespace cC_test
{

    public static class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Username:");

            SocketC s = new SocketC(Console.ReadLine());            
            
            s.OnMessageReceived += S_OnMessageReceived;
            s.OnConnected += S_OnConnected;
            s.OnDisconnected += S_OnDisconnected;
            s.OnChannelChanged += S_OnChannelChanged;
            s.OnConnectionCheckedAsync += S_OnConnectionCheckedAsync;
            s.OnConnectionFail += S_OnConnectionFail;
            s.Connect("127.0.0.1", 1236);
        }

        private static void S_OnMessageReceived(Message arg1, SocketC arg2)
        {
            Console.WriteLine(arg1.Body);
        }

        private static void S_OnConnectionFail(Exception obj)
        {
            Console.WriteLine("A conexão falhou");
        }

        private static void S_OnConnectionCheckedAsync(bool obj, DateTime d)
        {
            Console.WriteLine("conectado : " + obj + " em : " + d.ToString());
        }

        private static void S_OnSocketLeftInTheChannel(Message obj)
        {
            Console.WriteLine("usuario saiu do chanel");
        }

        private static void S_OnNewSocketEnterInTheChannel(Message obj)
        {
            Console.WriteLine("novo usuario no chanel"); 
        }

        private static void S_OnChannelChanged(Socket.Messages.Message arg1, SocketC arg2)
        {
            Console.WriteLine("trocou de chanel"); 
            Console.WriteLine($"foi para {arg1.Channel}"); 
        }

        private static void S_OnDisconnected(SocketC obj)
        {
            Console.WriteLine("disconectado");
        }

        private static void S_OnConnected(SocketC obj)
        {
            Console.WriteLine($"conectado como : {obj.UserName}#{obj.GUID}");
        }

        
       
    }

}
