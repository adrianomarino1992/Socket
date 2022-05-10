using MySocket.Client;
using MySocket.DTO;
using MySocket.Messages;
using MySocket.Messages.Body;

using System.Net;
using System.Net.Sockets;

namespace MySocket.Server
{
    public class SocketS
    {
        #region EVENTS

        public event Action<SocketC> OnClientAccepted;
        public event Action<SocketC> OnClientDisconnect;
        public event Action<MySocket.Server.SocketS> OnServerUp;
        public event Action<MySocket.Server.SocketS> OnServerDown;
        public event Action<MySocket.Server.SocketS> OnTryUpServerFail;
        public event Action<Message, SocketC> OnMessageReceived;
        public event Action<Exception> OnExceptionReceived;
        public event Action<ChangeChannelBody, SocketC> OnClientChangeChannel;
        #endregion


        #region PRIVATE FIELDS
        private TcpListener _tcpListener;
        private Thread _thread;
        private bool _keepRunning;
        private bool _isAlive { get => _thread != null && _thread.IsAlive; }
        #endregion


        #region PUBLIC PROPERTIES
        public string DefaultChannel { get => Configurations.Config.ServerName; }
        public bool Connected => m_connected();

        #endregion

#pragma warning disable
        public SocketS(IPEndPoint localEPt)
        {
#pragma warning enable
            _tcpListener = new TcpListener(localEPt);
        }

        #region CONNECTION METHODS
        public void Connect()
        {
            if (m_connected())
            {
                OnServerUp?.Invoke(this);
                return;
            }

            m_start();
        }

        public void Disconnect()
        {
            try
            {
                if (!m_connected())
                {
                    OnServerDown?.Invoke(this);
                    return;
                }

                _keepRunning = false;

                _tcpListener.Stop();

                OnServerDown?.Invoke(this);
            }
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);
            }

        }
        #endregion


        #region MESSAGE METHODS
        public void SendMessage(string codMsg, SocketC socket = null)
        {
            try
            {
                if (String.IsNullOrEmpty(codMsg))
                    throw new Exceptions.SocketException($"The message must be a valid text");


                MySocket.Channel.Channel.All.ForEach(d => d.BroadCast(codMsg, socket));
            }
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);
            }

        }
        #endregion

        #region PRIVATE METHODS
        private void m_start()
        {
            try
            {
                _tcpListener.Start();

                _keepRunning = true;

                MySocket.Channel.Channel.CreateChannel(DefaultChannel, this);

                _thread = new Thread(m_backGWork);

                _thread.IsBackground = true;

                _thread.Start();

                OnServerUp?.Invoke(this);
            }
            catch (Exception ex)
            {
                OnTryUpServerFail?.Invoke(this);
                OnExceptionReceived?.Invoke(ex);
            }

        }

        private bool m_connected()
        {
           return _tcpListener.Server.IsBound;
        }
        private void m_backGWork()
        {
            while (_isAlive && _keepRunning)
            {
                try
                {
                    SocketC incomming = new SocketC(_tcpListener.AcceptTcpClient());

                    incomming.OnMessageArriveFromClient += m_OnMessageArriveFromClient;
                    incomming.OnHandShakeDone += m_OnHandShakeDone;
                }
                catch (Exception ex)
                {
                    OnExceptionReceived?.Invoke(ex);
                }
            }

        }

        private void m_OnHandShakeDone(Message msg, SocketC socket)
        {
            try
            {
                if (!m_chekGuid(msg, socket))
                    return;
            }
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);
            }
        }


        private void m_OnMessageArriveFromClient(Message codMsg, SocketC socketC)
        {
            try
            {
                if (!m_checkPrivMssg(codMsg, socketC))
                    return;

                MySocket.Channel.Channel cliChannel = MySocket.Channel.Channel.All.FirstOrDefault(d => d.Contains(socketC));

                if (cliChannel != null)
                {
                    cliChannel.BroadCast(codMsg, socketC);
                }

                OnMessageReceived?.Invoke(codMsg, socketC);
            }
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);
            }
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

            if (codMsg.Header == Headers.CUSTOM_EVENT)
            {

                m_emitCEvt(codMsg, socketC);

                return false;
            }


            return true;
        }


        private void m_emitCEvt(Message msg, SocketC socket)
        {

            MySocket.Channel.Channel ch = MySocket.Channel.Channel.All.Where(d => d.Contains(socket)).FirstOrDefault();

            if (ch == null)
                return;

            Message emit = new Message
            {                               
                Event = msg.Event, 
                Header = Headers.CUSTOM_EVENT,
                FGUID = socket.GUID,
                From = socket.UserName,
                Channel = ch.Name, 
                ChannelsParts = ch.Participants.Select(d => new UserDTO { GUID = d.GUID, Name = d.UserName }).ToList(),
                Body = msg.Body
            };

            if(msg.Event.ToServer)
            {
                socket.SendMessage(emit);
            }
            else
            {
                ch.BroadCast(emit, socket);
            }
                        
        }


        private void m_sendAllPesChl(Message chgN, SocketC socket)
        {
            try
            {
                MySocket.Channel.Channel ch = MySocket.Channel.Channel.All.Where(d => d.Contains(socket)).FirstOrDefault();

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
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);
            }

        }

        private void m_sendPesChl(Message chgN, SocketC socket)
        {
            try
            {
                MySocket.Channel.Channel ch = MySocket.Channel.Channel.All.Where(d => d.Contains(socket)).FirstOrDefault();

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
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);
            }

        }
        private void m_chgChl(Message chgN, SocketC socket)
        {
            try
            {
                ChangeChannelBody ch = chgN.Body.FromJson<ChangeChannelBody>();

                MySocket.Channel.Channel.ChangeChannel(ch.From, ch.To, socket, this);

                OnClientChangeChannel?.Invoke(ch, socket);
            }
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);
            }

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
            try
            {
                MySocket.Channel.Channel.Disconnect(socketC);

                OnClientDisconnect?.Invoke(socketC);
            }
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);
            }
        }

        private bool m_chekGuid(Message message, SocketC socketC)
        {
            try
            {
                if (socketC.GUID == null)
                {
                    socketC.SetGuid(message.From, message.FGUID);
                    if (MySocket.Channel.Channel.All.Count == 0)
                        MySocket.Channel.Channel.CreateChannel(Configurations.Config.ServerName, this);

                    MySocket.Channel.Channel.All[0].Add(socketC);
                    socketC.SetChannel(MySocket.Channel.Channel.All[0].Name);
                    socketC.SendMessage(new Message { Header = Headers.SET_CHANNEL, Channel = socketC.Channel });

                    MySocket.Channel.Channel.All[0].InformNewUserIncomming(socketC);

                    OnClientAccepted?.Invoke(socketC);

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);

                return true;
            }

        } 
        #endregion
    }
}
