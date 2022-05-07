using Socket.Messages;
using System.Net;
using System.Net.Sockets;
using Socket;
using Socket.Messages.Body;
using Socket.Client;
using Socket.Channel;
using Socket.DTO;

namespace Socket.Server
{
    public class SocketS
    {

        public event Action<SocketC> OnClientAccepted;
        public event Action<SocketC> OnClientDisconnect;
        public event Action<Message, SocketC> OnMessageReceived;        
        public event Action<Exception> OnExceptionReceived;
        public event Action<ChangeChannelBody, SocketC> OnClientChangeChannel;

        private string _defaultChannel = $"Server";

        private TcpListener _tcpListener;

        private Thread _thread;
        private bool _isAlive { get => _thread != null && _thread.IsAlive; }        

        public string DefaultChannel { get => _defaultChannel; set => _defaultChannel = value; }

#pragma warning disable
        public SocketS(IPEndPoint localEPt)
        {
#pragma warning enable
            _tcpListener = new TcpListener(localEPt);
        }

        public void Connect()
        {
            try
            {
                m_start();
            }
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);
            }
        }



        public void SendMessage(string codMsg, SocketC socket = null)
        {
            if (String.IsNullOrEmpty(codMsg))
                throw new Exceptions.SocketException($"The message must be a valid text");


            Socket.Channel.Channel.All.ForEach(d => d.BroadCast(codMsg, socket));
           
        }

        private void m_start()
        {
            _tcpListener.Start();

            Socket.Channel.Channel.CreateChannel(_defaultChannel, this);

            _thread = new Thread(m_backGWork);

            _thread.IsBackground = true;

            _thread.Start();

        }

        public void m_backGWork()
        {
            while (_isAlive)
            {
                SocketC incomming = new SocketC(_tcpListener.AcceptTcpClient());

                incomming.OnMessageArriveFromClient += m_OnMessageArriveFromClient;                
                incomming.OnHandShakeDone += m_OnHandShakeDone; 
            }

        }

        private void m_OnHandShakeDone(Message msg, SocketC socket)
        {
            if (!m_chekGuid(msg, socket))
                return;
        }

        

        private void m_OnMessageArriveFromClient(Message codMsg, SocketC socketC)
        {            

            if (!m_checkPrivMssg(codMsg, socketC))
                return;

            Socket.Channel.Channel cliChannel = Socket.Channel.Channel.All.FirstOrDefault(d => d.Contains(socketC));

            if (cliChannel != null)
            {
                cliChannel.BroadCast(codMsg, socketC);
            }

            OnMessageReceived?.Invoke(codMsg, socketC);
        }

        private bool m_checkPrivMssg(Message codMsg, SocketC socketC)
        {
            if (codMsg.Header == Headers.DISCONNECT)
            {
                m_discCLi(socketC);

                return false;
            }

            if (codMsg.Header == Headers.CHANGE_CHANNEL)
            {               

                m_chgChl(codMsg, socketC);

                return false;
            }

            if (codMsg.Header == Headers.GET_PARTS_CHANNEL)
            {

                m_sendPesChl(codMsg, socketC);

                return false;
            }

            if (codMsg.Header == Headers.GET_ALL_CHANNEL)
            {

                m_sendAllPesChl(codMsg, socketC);

                return false;
            }


            return true;
        }

        private void m_sendAllPesChl(Message chgN, SocketC socket)
        {
            Socket.Channel.Channel ch = Socket.Channel.Channel.All.Where(d => d.Contains(socket)).FirstOrDefault();

            if (ch == null)
                return;

            RequestChannelsInfoBody body = new RequestChannelsInfoBody();
            body.Channels = new List<ChannelDTO>();
            body.Channels = Channel.Channel.All.Select(d => new ChannelDTO { Name = d.Name, Users = d.Participants.Select(p => new UserDTO { Name = p.UserName, GUID = p.GUID }).ToList() }).ToList();

            Message msg = new Message
            {
                ChannelsParts = ch.Participants.Select(d => new UserDTO { GUID = d.GUID, Name = d.UserName }).ToList(),
                Channel = ch.Name,
                Header = Headers.GET_ALL_CHANNEL,
                Body = body.ToJson()
                
            };

            socket.SendMessage(msg);

        }

        private void m_sendPesChl(Message chgN, SocketC socket)
        {
            Socket.Channel.Channel ch = Socket.Channel.Channel.All.Where(d => d.Contains(socket)).FirstOrDefault();

            if (ch == null)
                return;

            Message msg = new Message
            {
                ChannelsParts = ch.Participants.Select(d => new UserDTO { GUID = d.GUID, Name = d.UserName }).ToList(),
                Channel = ch.Name,
                Header = Headers.GET_PARTS_CHANNEL
            };

            socket.SendMessage(msg);            

        }
        private void m_chgChl(Message chgN, SocketC socket)
        {

            ChangeChannelBody ch = chgN.Body.FromJson<ChangeChannelBody>();

            Socket.Channel.Channel.ChangeChannel(ch.From, ch.To, socket, this);

            OnClientChangeChannel?.Invoke(ch, socket);
            
        }

        private Message m_crtMsg()
        {
            return new Message
            {
                DateTime = DateTime.Now,
                From = Configurations.Config.ServerName,
                FGUID = Configurations.Config.ServerGUID
            };
        }

        private void m_discCLi(SocketC socketC)
        {
            Socket.Channel.Channel.Disconnect(socketC);

            OnClientDisconnect?.Invoke(socketC);
        }

        private bool m_chekGuid(Message message, SocketC socketC)
        {
            if (socketC.GUID == null)
            {                
                socketC.SetGuid(message.From, message.FGUID);
                if (Socket.Channel.Channel.All.Count == 0)
                    Socket.Channel.Channel.CreateChannel(Configurations.Config.ServerName, this);

                Socket.Channel.Channel.All[0].Add(socketC);
                socketC.SetChannel(Socket.Channel.Channel.All[0].Name);
                socketC.SendMessage(new Message { Header = Headers.SET_CHANNEL, Channel = socketC.Channel });

                Socket.Channel.Channel.All[0].InformNewUserIncomming(socketC);

                OnClientAccepted?.Invoke(socketC);

                return false;
            }

            return true;

        }
    }
}
