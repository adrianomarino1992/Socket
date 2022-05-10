using MySocket.DTO;
using MySocket.Messages;
using MySocket.Messages.Body;
using Socket.Objects.Events;
using System.Net.Sockets;


namespace MySocket.Client
{
    public class SocketC
    {

        #region EVENTS
        public event Action<Message, SocketC> OnMessageReceived;
        public event Action<UserEnterOrLeaveTheChannelBody, List<UserDTO>> OnNewSocketEnterInTheChannel;
        public event Action<UserEnterOrLeaveTheChannelBody, List<UserDTO>> OnSocketLeftInTheChannel;
        public event Action<Message, SocketC> OnHandShakeDone;
        public event Action<SocketC> OnConnected;
        public event Action<SocketC> OnReconnected;
        public event Action<SocketC> OnReconnectFail;
        public event Action<SocketC> OnDisconnected;
        public event Action<Exception> OnConnectionFail;
        public event Action<Message, SocketC> OnChannelChanged;
        public event Action<RequestChannelsInfoBody, SocketC> OnReceiveChannelsInfo;
        public event Action<List<UserDTO>, string, SocketC> OnPartsOfChannelUpdated;
        public event Action<bool, DateTime> OnConnectionCheckedAsync;
        
        
        #endregion

        #region INTERNAL EVENTS
        internal event Action<Message, SocketC> OnMessageArriveFromClient;
        #endregion


        #region PRIVATE FIELDS
        private TcpClient _tcpClient;
        private NetworkStream _network;
        private bool _isAlive { get => _thread != null && _thread.IsAlive; }
        private Thread _thread;
        private string _username;
        private string _guid;
        private string _host;
        private int _port;
        private bool _canConnect;
        private string _channel;
        private DateTime _lastPing = DateTime.Now;
        private bool _lastStatus;
        private bool _threadKill;
        private int _checkConnTimeSpan = 5;
        private List<Socket.Objects.Events.Event> _events = new List<Socket.Objects.Events.Event>();
        #endregion

        #region PUBLIC PROPERTIES
        public string UserName { get => _username; set => _username = value; }
        public string GUID { get => _guid; set => _guid = value; }
        public string Host { get => _host; }
        public string Channel { get => _channel; }
        public int Port { get => _port; }
        public bool Connected => m_connected();
        public int CheckConectInterval { get => _checkConnTimeSpan; set { if (value >= 5) _checkConnTimeSpan = value; } } 
        #endregion




#pragma warning disable
        public SocketC(string userName)
        {
#pragma warning enable
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentNullException("The username is required");

            _username = userName;
            m_guid();
            _tcpClient = new TcpClient();
            _canConnect = true;
            
        }

#pragma warning disable
        public SocketC(TcpClient tcpClient)
        {
#pragma warning enable
            if (!tcpClient.Connected)
                throw new MySocket.Exceptions.SocketConnectionException("The socket is not connected with the server");
            _threadKill = false;
            _tcpClient = tcpClient;
            _network = tcpClient.GetStream();
            _canConnect = false;            
            m_start();

        }

        #region CONNECTION METHODS
        public void Connect(string host, int port)
        {
            if (!_canConnect)
                throw new MySocket.Exceptions.SocketConnectionException("The socket is already connected with the server");
            try
            {
                _threadKill = false;
                _tcpClient.Connect(host, port);
                OnConnected?.Invoke(this);
            }
            catch (Exception ex)
            {
                OnConnectionFail?.Invoke(ex);
                return;
            }

            _network = _tcpClient.GetStream();
            _host = host;
            _port = port;
            m_start();
            m_handShake();
        }

        public void Reconnect(int times)
        {
            Disconnect();

            _ = Task.Run(async () =>
            {
                int cur = 0;

                while (!m_connected() && cur < times)
                {
                    Task.Delay(1000);

                    Connect(_host, _port);

                    cur++;
                }

                if (m_connected())
                {
                    OnReconnected?.Invoke(this);
                }
                else
                {
                    OnReconnectFail?.Invoke(this);
                }
            });


        }


        public void Disconnect()
        {
            if (m_connected())
            {
                Message msg = m_crMessage("Disconnect");
                msg.Header = Headers.DISCONNECT;
                SendMessage(msg);
            }

            if (_thread == null || _tcpClient == null)
                return;

            _tcpClient.Close();

            try
            {
                _thread.Abort();

            }
            catch
            {

            }
            _threadKill = true;
            _tcpClient = new TcpClient();
            _canConnect = true;
            OnDisconnected?.Invoke(this);
        }
        #endregion

        #region MESSAGE METHODS
        public void SendMessageTo(string msg, string Uid)
        {
            try
            {
                if (_isAlive)
                {
                    Message m = m_crMessage(msg);
                    m.TGUID = Uid;
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(m.ToString());
                    _network.Write(data, 0, data.Length);
                }
            }
            catch
            {

            }
        }


        public void SendMessage(string msg)
        {
            try
            {
                if (_isAlive)
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(m_crMessage(msg).ToString());
                    _network.Write(data, 0, data.Length);
                }
            }
            catch
            {

            }
        }

        public void SendMessage(Message msg)
        {
            try
            {
                if (_isAlive)
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(msg.ToString());
                    _network.Write(data, 0, data.Length);
                }
            }
            catch
            {

            }
        }
        #endregion

        #region PUBLIC METHODS


        public void On(string @event, Action<Message> action, bool @continue = true, bool @onlyFromserver = true)
        {            
            _events.Add(new Event(@event, action, @continue, !onlyFromserver));
        }

        public void Emit(string @event, string msgStr, bool @public = false)
        {
            Message msg = m_crMessage(msgStr);
            msg.Event = new Socket.DTO.EventDTO{ Name = @event, ToServer = !@public };
            msg.Header = Headers.CUSTOM_EVENT;  
            SendMessage(msg);
        }

        public void Blur(string @event)
        {
            _events.RemoveAll(d => d.Name == @event);
        }

        public void SetGuid(string uName, string uGuid)
        {
            _username = uName;
            _guid = uGuid;
        }
        public void SetChannel(string cName)
        {
            _channel = cName;

        }

        public void RequestOthersPartsOfChannel()
        {
            Message message = m_crMessage("see all");
            message.Channel = _channel;
            message.Header = Headers.GET_PARTS_CHANNEL;
            SendMessage(message);
        }

        public void RequestAllChannels()
        {
            Message message = m_crMessage("get all");
            message.Channel = _channel;
            message.Header = Headers.GET_ALL_CHANNEL;
            SendMessage(message);
        }

        public void ChangeChannel(string chnName)
        {
            Message message = m_crMessage("change channel");
            message.Channel = chnName;
            message.Header = Headers.CHANGE_CHANNEL;
            message.Body = new Messages.Body.ChangeChannelBody { From = _channel, To = chnName }.ToJson();
            SendMessage(message);
        }
        #endregion



        #region PRIVATE METHODS
        private void m_start()
        {
            if (_thread != null && _thread.IsAlive)
                throw new MySocket.Exceptions.SocketConnectionException("The server is already started");

            _thread = new Thread(m_readNetWork);
            _thread.IsBackground = true;
            _thread.Start();
        }

        private string m_guid()
        {
            _guid = new string(Guid.NewGuid().ToString().Take(5).ToArray());

            return _guid;
        }

        private Message m_crMessage(string msg)
        {
            return new Message
            {
                From = _username,
                Channel = _channel,
                FGUID = _guid,
                DateTime = DateTime.Now,
                Header = Headers.DEFAULT_MESSAGE,
                Body = msg
            };
        }

        private void m_handShake()
        {
            try
            {
                if (_isAlive)
                {
                    Message hd = new Message
                    {
                        From = _username,
                        Channel = _channel,
                        FGUID = _guid,
                        DateTime = DateTime.Now,
                        Header = Headers.HANDSHAKE,
                        Body = "handshake"
                    };
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(
                       hd.ToString());

                    _network.Write(data, 0, data.Length);

                    OnHandShakeDone?.Invoke(hd, this);
                }
            }
            catch
            {

            }

        }

        private void m_readNetWork()
        {

            while (_isAlive)
            {
                if (_threadKill)
                    goto Exit;
                try
                {
                    m_dsCon();

                    if (_network.DataAvailable)
                    {
                        byte[] data = new byte[_tcpClient.Available];

                        _network.Read(data, 0, data.Length);

                        string msg = System.Text.Encoding.UTF8.GetString(data);

                        OnMessageArriveFromClient?.Invoke(Message.ToMessage(msg), this);

                        if (m_checkPrivMssg(Message.ToMessage(msg)))
                        {
                            OnMessageReceived?.Invoke(Message.ToMessage(msg), this);
                        }

                    }
                }
                catch
                {

                }
            }

        Exit:
            {
                _threadKill = false;
            }
        }

        private bool m_checkPrivMssg(Message msg)
        {
            if (!m_checkHandS(msg))
                return false;

            if (!m_setChanel(msg))
                return false;

            if (!m_changeChanel(msg))
                return false;

            if (!m_NuInChanel(msg))
                return false;

            if (!m_NuOutChanel(msg))
                return false;

            if (!m_PartsChannelUp(msg))
                return false;

            if (!m_reqAllChannels(msg))
                return false;

            if (!m_evtFilter(msg))
                return false;

            return true;
        }

        private bool m_evtFilter(Message msg)
        {
            if(msg.Event == null || string.IsNullOrEmpty(msg.Event.Name))
            {
                return true;
            }

            List<Event> events = _events.Where(d => d.Name == msg.Event.Name).ToList();

            bool r = events.All(d => d.Continue);

            events.ForEach(d => {
                
                if(!d.OnlyFromServer || msg.FGUID == _guid)
                {
                    d.Execute(msg);
                }
            });

            return r;

        }

        private void m_dsCon()
        {
            if (DateTime.Now.Subtract(_lastPing).TotalSeconds > _checkConnTimeSpan)
            {
                _lastPing = DateTime.Now;
                _lastStatus = m_connected();
                OnConnectionCheckedAsync?.Invoke(_lastStatus, _lastPing);
            }
        }
        private bool m_connected()
        {
            if (_network == null)
                return false;
            try
            {
                bool s = _network.Socket.Poll(1000, SelectMode.SelectRead);
                bool d = (_network.Socket.Available == 0);
                if (s && d)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }


        private bool m_NuInChanel(Message msg)
        {
            if (msg.Header == Headers.USER_ENTERED_ROOM)
            {
                UserEnterOrLeaveTheChannelBody body = msg.Body.FromJson<UserEnterOrLeaveTheChannelBody>();
                OnNewSocketEnterInTheChannel?.Invoke(body, msg.ChannelsParts);

                return false;
            }

            return true;
        }

        private bool m_PartsChannelUp(Message msg)
        {
            if (msg.Header == Headers.GET_PARTS_CHANNEL)
            {
                OnPartsOfChannelUpdated?.Invoke(msg.ChannelsParts, msg.Channel, this);

                return false;
            }

            return true;
        }

        private bool m_NuOutChanel(Message msg)
        {
            if (msg.Header == Headers.USER_LEFT_ROOM)
            {
                UserEnterOrLeaveTheChannelBody body = msg.Body.FromJson<UserEnterOrLeaveTheChannelBody>();

                OnSocketLeftInTheChannel?.Invoke(body, msg.ChannelsParts);

                return false;
            }

            return true;
        }


        private bool m_reqAllChannels(Message msg)
        {
            if (msg.Header == Headers.GET_ALL_CHANNEL)
            {
                OnReceiveChannelsInfo?.Invoke(msg.Body.FromJson<RequestChannelsInfoBody>(), this);

                return false;
            }

            return true;
        }

        private bool m_changeChanel(Message msg)
        {
            if (msg.Header == Headers.CHANGE_CHANNEL)
            {
                _channel = msg.Channel;

                OnChannelChanged?.Invoke(msg, this);

                return false;
            }

            return true;
        }

        private bool m_setChanel(Message msg)
        {
            if (msg.Header == Headers.SET_CHANNEL)
            {
                _channel = msg.Channel;

                return false;
            }

            return true;
        }

        private bool m_checkHandS(Message msg)
        {
            if (msg.Header == Headers.HANDSHAKE)
            {
                _channel = msg.Channel;

                OnHandShakeDone?.Invoke(msg, this);

                return false;
            }

            return true;
        } 
        #endregion
    }
}
