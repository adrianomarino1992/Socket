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
            s.OnNewSocketEnterInTheChannel += S_OnNewSocketEnterInTheChannel;
            s.OnSocketLeftInTheChannel += S_OnSocketLeftInTheChannel;
            s.OnConnectionCheckedAsync += S_OnConnectionCheckedAsync;
            s.OnConnectionFail += S_OnConnectionFail;
            s.Connect("127.0.0.1", 1236);

            while(true)
            {
                string ss = Console.ReadLine();

                if (ss == "s")
                {
                    s.Disconnect();

                    Console.ReadLine();

                    goto Exit;
                }

                if (ss == "c")
                {
                    s.ChangeChannel("novo");

                    goto Exit;

                }

                if (ss.StartsWith("-p"))
                {
                    s.SendMessageTo("privadinhaaa", ss.Replace("-p",""));

                    goto Exit;

                }

                if (ss.StartsWith("-e"))
                {
                    s.Disconnect();
                    s.Connect("127.0.0.1", 1236);

                    goto Exit;

                }

                s.SendMessage(ss);

                Exit: 
                
                {
                    
                }
            }


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

        private static void S_OnMessageReceived(Message arg1,SocketC arg2)
        {
            Console.WriteLine(arg1.Body);
        }
       
    }

}
