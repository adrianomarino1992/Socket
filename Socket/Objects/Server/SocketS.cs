using Socket.Client;
using Socket.DTO;
using Socket.Messages;
using Socket.Messages.Body;

using System.Net;
using System.Net.Sockets;

namespace Socket.Server
{
    public class SocketS
    {
        #region EVENTS

        public event Action<SocketC> OnClientAccepted;
        public event Action<SocketC> OnClientDisconnect;
        public event Action<Socket.Server.SocketS> OnServerUp;
        public event Action<Socket.Server.SocketS> OnServerDown;
        public event Action<Socket.Server.SocketS> OnTryUpServerFail;
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


                Socket.Channel.Channel.All.ForEach(d => d.BroadCast(codMsg, socket));
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

                Socket.Channel.Channel.CreateChannel(DefaultChannel, this);

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

                Socket.Channel.Channel cliChannel = Socket.Channel.Channel.All.FirstOrDefault(d => d.Contains(socketC));

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


            return true;
        }

        private void m_sendAllPesChl(Message chgN, SocketC socket)
        {
            try
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
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);
            }

        }

        private void m_sendPesChl(Message chgN, SocketC socket)
        {
            try
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

                Socket.Channel.Channel.ChangeChannel(ch.From, ch.To, socket, this);

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
                Socket.Channel.Channel.Disconnect(socketC);

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
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);

                return true;
            }

        } 
        #endregion
    }
}
