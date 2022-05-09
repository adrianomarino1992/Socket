using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Socket.Messages;
using Socket.Client;
using Socket.Server;
using Socket.DTO;

namespace Socket.Channel
{
    public class Channel
    {

        public static List<Channel> All { get; set; } = new List<Channel>();

        public static Channel CreateChannel(string name, SocketS server)
        {
            if (Channel.All.Any(d => d.Name.ToLower().Trim() == name.ToLower().Trim()))
            {
                return Channel.All.First(d => d.Name.ToLower().Trim() == name.ToLower().Trim());
            }

            Channel ch = new Channel(name, server);

            All.Add(ch);

            return ch;
        }

        public static Channel ChangeChannel(string pre, string chgN, SocketC socket, SocketS server)
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

        public static void Disconnect(SocketC socketC)
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

        private List<SocketC> _clients;

        public List<SocketC> Participants { get => _clients; }

        private SocketS _server;

        private string _name;
        public string Name { get => _name; }

#pragma warning disable
        private Channel(string name, SocketS server)
#pragma warning enable
        {
            _name = name;
            _clients = new List<SocketC>();
            _server = server;
        }

        public bool Contains(SocketC socket)
        {
            return _clients.Any(d => d.GUID == socket.GUID);
        }

        public SocketC Add(SocketC socket)
        {
            if (!_clients.Any(d => d.GUID == socket.GUID))
                _clients.Add(socket);

            return socket;
        }

        public void InformNewUserIncomming(SocketC socketC)
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

        public void InformNewUserLeaving(SocketC socketC)
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

        public SocketC Remove(SocketC socket)
        {
            if (_clients.Any(d => d.GUID == socket.GUID))
                _clients.RemoveAll(d => d.GUID == socket.GUID);

            return socket;
        }

        public void BroadCast(Message codMsg, SocketC sender = null)
        {
            codMsg.ChannelsParts = _clients.Select(d => new UserDTO { GUID = d.GUID.Trim(), Name = d.UserName }).ToList();
            codMsg.Channel = this.Name;

            List<SocketC> tgts = _clients;

            if (!String.IsNullOrEmpty(codMsg.TGUID))
                tgts = _clients.Where(d => d.GUID.Trim() == codMsg.TGUID.Trim()).ToList();

            foreach (SocketC sc in tgts)
            {
                if(sc != null && sc.GUID.Trim() != sender.GUID.Trim())
                    sc.SendMessage(codMsg);
            }

        }

        public void BroadCast(string codMsg, SocketC sender = null)
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

            foreach (SocketC sc in _clients)
            {
                if(sc != null && sc.GUID != sender.GUID)
                {                    
                    sc.SendMessage(co);
                }
                    
            }

        }
    }
}
