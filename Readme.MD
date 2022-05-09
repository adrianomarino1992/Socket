# Adr.MySockets

Adr.MySockets is a package with classes used to facilitate the websocket communication in C#.


## Installation

.NET CLI

```bash
dotnet add package Adr.MySockets --version 1.0.0
```

Nuget package manager

```bash
PM> Install-Package Adr.MySockets -Version 1.0.0
```

packageReference

```bash
<PackageReference Include="Adr.MySockets" Version="1.0.0" />
```

## Sample of basic events only

**Create a simple server:**
```csharp
        public static void Main(string[] args) {

            SocketS server = new SocketS(IPEndPoint.Parse("127.0.0.1:1236"));    
            
            server.OnClientAccepted += S_OnClientAccepted;
            server.OnMessageReceived += S_OnMessageReceived;
            server.OnClientDisconnect += S_OnClientDisconnect;            
            server.OnServerUp += S_OnServerUp;  
            
            server.Connect();

            Console.ReadKey();  
        
        }

        private static void S_OnServerUp(SocketS obj) => Console.WriteLine($"Server online");
        private static void S_OnClientDisconnect(SocketC obj) => Console.WriteLine("Client disconnected: " + obj.UserName);        
        private static void S_OnMessageReceived(Message arg1, SocketC arg2) => Console.WriteLine("Message: " + arg1);
        private static void S_OnClientAccepted(SocketC obj) => Console.WriteLine($"Client connected: " + obj.UserName); 
```


**Create a simple client:**
```csharp
        public static void Main(string[] args) {

            Console.WriteLine("Username:");
            SocketC cli = new SocketC(Console.ReadLine());

            cli.OnConnected += Cli_OnConnected;
            cli.OnConnectionFail += Cli_OnConnectionFail;
            cli.OnMessageReceived += Cli_OnMessageReceived;
            cli.Connect("127.0.0.1", 1236);

            while (true)
            {
                cli.SendMessage(Console.ReadLine());
            }
        
        }

        private static void Cli_OnMessageReceived(Message arg1, SocketC arg2) => Console.WriteLine($"{arg1.From}: {arg1.Body}");
        private static void Cli_OnConnectionFail(Exception obj) => Console.WriteLine($"Error on connection: {obj.Message}");
        private static void Cli_OnConnected(SocketC obj) => Console.WriteLine("Connected!");
```
## Check the complete chat app project 
**GitHub link : https://github.com/adrianomarino1992/Socket**
[![Alt text](https://raw.githubusercontent.com/adrianomarino1992/Socket/master/t.png)](https://raw.githubusercontent.com/adrianomarino1992/Socket/master/t.png)



## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

##  Git

https://github.com/adrianomarino1992/MyORMForMySQL.git
