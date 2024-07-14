using MySocket.Client;
using MySocket.DTO;
using MySocket.Messages;
using MySocket.Server;

namespace MySocket.Channel
{
    public class Channel
    {

        public static List<Channel> All { get; set; } = new List<Channel>();

        public static Channel CreateChannel(string name, SocketServer server)
        {
            if (Channel.All.Any(d => d.Name.ToLower().Trim() == name.ToLower().Trim()))
            {
                return Channel.All.First(d => d.Name.ToLower().Trim() == name.ToLower().Trim());
            }

            Channel ch = new Channel(name, server);

            All.Add(ch);

            return ch;
        }

        public static Channel ChangeChannel(string pre, string chgN, SocketClient socket, SocketServer server)
        {
            try
            {
                if (!Channel.ChannelExists(chgN))
                    Channel.CreateChannel(chgN, server);
            }
            catch { }

            Channel curr = Channel.All.Where(d => d.Name == chgN).First();


            curr.Add(socket);

            if (!String.IsNullOrEmpty(pre) && pre != chgN && Channel.ChannelExists(pre))
            {
                Channel prevC = Channel.All.Where(d => d.Name == pre).First();
                prevC.Remove(socket);
                prevC.InformNewUserLeaving(socket);

                if (prevC._clients.Count == 0)
                    Channel.DeleteChannel(prevC.Name);

            }


            Message msg = new Message { DateTime = DateTime.Now, From = Configurations.Config.ServerName, FGUID = Configurations.Config.ServerGUID };
            msg.Channel = curr.Name;
            msg.Header = Headers.CHANGE_CHANNEL;
            msg.ChannelsParts = curr.Participants.Select(d => new UserDTO { GUID = d.GUID, Name = d.UserName }).ToList();

            socket.SendMessage(msg);

            curr.InformNewUserIncomming(socket);

            return curr;
        }

        public static bool ChannelExists(string name)
        {
            return Channel.All.Any(d => d.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public static void DeleteChannel(string name)
        {
            if (!Channel.All.Any(d => d.Name.ToLower().Trim() == name.ToLower().Trim()))
            {
                throw new Exceptions.ChannelException($"Channel {name} not exists");
            }

            All.RemoveAll(d => d.Name.ToLower().Trim() == name.ToLower().Trim());

        }

        public static void Disconnect(SocketClient socketC)
        {
            if (Channel.ChannelExists(socketC.Channel))
            {
                Channel curr = Channel.All.Where(d => d.Name == socketC.Channel).First();

                curr.Remove(socketC);

                curr.InformNewUserLeaving(socketC);

                if (curr._clients.Count == 0)
                    Channel.DeleteChannel(curr.Name);
            }
        }

        private List<SocketClient> _clients;

        public List<SocketClient> Participants { get => _clients; }

        private SocketServer _server;

        private string _name;
        public string Name { get => _name; }

#pragma warning disable
        private Channel(string name, SocketServer server)
#pragma warning enable
        {
            _name = name;
            _clients = new List<SocketClient>();
            _server = server;
        }

        public bool Contains(SocketClient socket)
        {
            return _clients.Any(d => d.GUID == socket.GUID);
        }

        public SocketClient Add(SocketClient socket)
        {
            if (!_clients.Any(d => d.GUID == socket.GUID))
                _clients.Add(socket);

            return socket;
        }

        public void InformNewUserIncomming(SocketClient socketC)
        {

            BroadCast(
                    new Message
                    {
                        From = socketC.UserName,
                        FGUID = socketC.GUID,
                        DateTime = DateTime.Now,
                        Body = new Messages.Body.UserEnterOrLeaveTheChannelBody() { Channel = Name, Name = socketC.UserName, GUID = socketC.GUID }.ToJson(),
                        Header = Headers.USER_ENTERED_ROOM
                    }, socketC);
        }

        public void InformNewUserLeaving(SocketClient socketC)
        {

            BroadCast(
                    new Message
                    {
                        From = socketC.UserName,
                        FGUID = socketC.GUID,
                        DateTime = DateTime.Now,
                        Body = new Messages.Body.UserEnterOrLeaveTheChannelBody() { Channel = Name, Name = socketC.UserName, GUID = socketC.GUID }.ToJson(),
                        Header = Headers.USER_LEFT_ROOM
                    }, socketC);
        }

        public SocketClient Remove(SocketClient socket)
        {
            if (_clients.Any(d => d.GUID == socket.GUID))
                _clients.RemoveAll(d => d.GUID == socket.GUID);

            return socket;
        }

        public void BroadCast(Message codMsg, SocketClient sender = null)
        {
            codMsg.ChannelsParts = _clients.Select(d => new UserDTO { GUID = d.GUID.Trim(), Name = d.UserName }).ToList();
            codMsg.Channel = this.Name;

            List<SocketClient> tgts = _clients;

            if(sender != null && !String.IsNullOrEmpty(sender.GUID))
            {
                codMsg.FGUID = sender.GUID.Trim();
                codMsg.From = sender.UserName.Trim();
            }

            if (!String.IsNullOrEmpty(codMsg.TGUID))
                tgts = _clients.Where(d => d.GUID.Trim() == codMsg.TGUID.Trim()).ToList();

            foreach (SocketClient sc in tgts)
            {
                if (sc != null && sc.GUID.Trim() != sender.GUID.Trim())
                    sc.SendMessage(codMsg);
            }

        }

        public void BroadCast(string codMsg, SocketClient sender = null)
        {
            Message co = new Message
            {
                From = sender != null ? sender.UserName : "server",
                Channel = this.Name,
                FGUID = sender != null ? sender.GUID : "server",
                DateTime = DateTime.Now,
                Header = Headers.DEFAULT_MESSAGE,
                Body = codMsg
            };


            co.ChannelsParts = _clients.Select(d => new UserDTO { GUID = d.GUID, Name = d.UserName }).ToList();

            foreach (SocketClient sc in _clients)
            {
                if (sc != null && sc.GUID != sender.GUID)
                {
                    sc.SendMessage(co);
                }

            }

        }
    }
}
