using MySocket.DTO;
using MySocket.Messages;
using MySocket.Messages.Body;
using Socket.Objects.Events;
using System.Net.Sockets;


namespace MySocket.Client
{
    public class SocketClient
    {

        #region EVENTS
        public event Action<Message, SocketClient> OnMessageReceived;
        public event Action<UserEnterOrLeaveTheChannelBody, List<UserDTO>> OnNewSocketEnterInTheChannel;
        public event Action<UserEnterOrLeaveTheChannelBody, List<UserDTO>> OnSocketLeftInTheChannel;
        public event Action<Message, SocketClient> OnHandShakeDone;
        public event Action<SocketClient> OnConnected;
        public event Action<SocketClient> OnReconnected;
        public event Action<SocketClient> OnReconnectFail;
        public event Action<SocketClient> OnDisconnected;
        public event Action<Exception> OnConnectionFail;
        public event Action<Exception> OnReadNetWorkFail;
        public event Action<Message, SocketClient> OnChannelChanged;
        public event Action<RequestChannelsInfoBody, SocketClient> OnReceiveChannelsInfo;
        public event Action<List<UserDTO>, string, SocketClient> OnPartsOfChannelUpdated;
        public event Action<bool, DateTime> OnConnectionCheckedAsync;
        
        
        #endregion

        #region INTERNAL EVENTS
        internal event Action<Message, SocketClient> OnMessageArriveFromClient;
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
        public Socket.Objects.Enumerables.NetWorkImportance NetWorkImportance { get; set; } = Socket.Objects.Enumerables.NetWorkImportance.HIGH;
        private int _checkConnTimeSpan = 5;
        private List<Socket.Objects.Events.Event> _events = new List<Socket.Objects.Events.Event>();
        #endregion

        #region PUBLIC PROPERTIES
        public string UserName { get => _username; set => _username = value; }
        public string GUID { get => _guid; set => _guid = value; }
        public string Host { get => _host; }
        public string Channel { get => _channel; }
        public int Port { get => _port; }
        public bool Connected => IsConnected();
        public int CheckConectInterval { get => _checkConnTimeSpan; set { if (value >= 5) _checkConnTimeSpan = value; } } 
        #endregion




#pragma warning disable
        public SocketClient(string userName)
        {
#pragma warning enable
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentNullException("The username is required");

            _username = userName;
            CreateGUID();
            _tcpClient = new TcpClient();
            _canConnect = true;
            
        }

#pragma warning disable
        public SocketClient(TcpClient tcpClient)
        {
#pragma warning enable
            if (!tcpClient.Connected)
                throw new MySocket.Exceptions.SocketConnectionException("The socket is not connected with the server");
            _threadKill = false;
            _tcpClient = tcpClient;
            _network = tcpClient.GetStream();
            _canConnect = false;            
            Start();

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
            Start();
            DoHandShake();
        }

        public void Reconnect(int times)
        {
            Disconnect();

            _ = Task.Run(async () =>
            {
                int cur = 0;

                while (!IsConnected() && cur < times)
                {
                    Task.Delay(1000);

                    Connect(_host, _port);

                    cur++;
                }

                if (IsConnected())
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
            if (IsConnected())
            {
                Message msg = CreateMessage("Disconnect");
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
                    Message m = CreateMessage(msg);
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
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(CreateMessage(msg).ToString());
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
            Message msg = CreateMessage(msgStr);
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
            Message message = CreateMessage("see all");
            message.Channel = _channel;
            message.Header = Headers.GET_PARTS_CHANNEL;
            SendMessage(message);
        }

        public void RequestAllChannels()
        {
            Message message = CreateMessage("get all");
            message.Channel = _channel;
            message.Header = Headers.GET_ALL_CHANNEL;
            SendMessage(message);
        }

        public void ChangeChannel(string chnName)
        {
            Message message = CreateMessage("change channel");
            message.Channel = chnName;
            message.Header = Headers.CHANGE_CHANNEL;
            message.Body = new Messages.Body.ChangeChannelBody { From = _channel, To = chnName }.ToJson();
            SendMessage(message);
        }
        #endregion



        #region PRIVATE METHODS
        private void Start()
        {
            if (_thread != null && _thread.IsAlive)
                throw new MySocket.Exceptions.SocketConnectionException("The server is already started");

            Socket.Objects.Enumerables.NetWorkImportance curr = NetWorkImportance;
            NetWorkImportance = Socket.Objects.Enumerables.NetWorkImportance.VERYHIGH;

            _thread = new Thread(HandleNetworkStream);
            _thread.IsBackground = true;
            _thread.Start();

            _ = Task.Run(async () => { 
                
                await Task.Delay(1000);
                NetWorkImportance = curr;
            });
        }

        private string CreateGUID()
        {
            _guid = new string(Guid.NewGuid().ToString().Take(5).ToArray());

            return _guid;
        }

        private Message CreateMessage(string msg)
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

        private void DoHandShake()
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

        private void HandleNetworkStream()
        {

            while (_isAlive)
            {
                if (_threadKill)
                    goto Exit;
                try
                {
                    CheckConnection();

                    Thread.Sleep((int)NetWorkImportance);
                    
                    List<Byte> data = new List<Byte>();

                    while (_network.DataAvailable) 
                    {
                        byte[] chunk = new byte[_tcpClient.Available];

                        _network.Read(chunk, 0, chunk.Length);

                        data.AddRange(chunk); 
                    }

                    if(data.Count > 0)
                    {
                        string msg = System.Text.Encoding.UTF8.GetString(data.ToArray());

                        OnMessageArriveFromClient?.Invoke(Message.ToMessage(msg), this);

                        if (HandleInternalMessage(Message.ToMessage(msg)))
                        {
                            OnMessageReceived?.Invoke(Message.ToMessage(msg), this);
                        }
                    }
                }
                catch(Exception ex)
                {
                    OnReadNetWorkFail?.Invoke(ex);
                }
            }

        Exit:
            {
                _threadKill = false;
            }
        }



        private bool HandleInternalMessage(Message msg)
        {
            if (!CheckHandShake(msg))
                return false;

            if (!SetChannel(msg))
                return false;

            if (!ChangeChannel(msg))
                return false;

            if (!NewUserInChannel(msg))
                return false;

            if (!LeftChannel(msg))
                return false;

            if (!PartsChannelUpdate(msg))
                return false;

            if (!RequestAllChannels(msg))
                return false;

            if (!EventFilter(msg))
                return false;

            return true;
        }

        private bool EventFilter(Message msg)
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

        private void CheckConnection()
        {
            if (DateTime.Now.Subtract(_lastPing).TotalSeconds > _checkConnTimeSpan)
            {
                _lastPing = DateTime.Now;
                _lastStatus = IsConnected();
                OnConnectionCheckedAsync?.Invoke(_lastStatus, _lastPing);
            }
        }
        private bool IsConnected()
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


        private bool NewUserInChannel(Message msg)
        {
            if (msg.Header == Headers.USER_ENTERED_ROOM)
            {
                UserEnterOrLeaveTheChannelBody body = msg.Body.FromJson<UserEnterOrLeaveTheChannelBody>();
                OnNewSocketEnterInTheChannel?.Invoke(body, msg.ChannelsParts);

                return false;
            }

            return true;
        }

        private bool PartsChannelUpdate(Message msg)
        {
            if (msg.Header == Headers.GET_PARTS_CHANNEL)
            {
                OnPartsOfChannelUpdated?.Invoke(msg.ChannelsParts, msg.Channel, this);

                return false;
            }

            return true;
        }

        private bool LeftChannel(Message msg)
        {
            if (msg.Header == Headers.USER_LEFT_ROOM)
            {
                UserEnterOrLeaveTheChannelBody body = msg.Body.FromJson<UserEnterOrLeaveTheChannelBody>();

                OnSocketLeftInTheChannel?.Invoke(body, msg.ChannelsParts);

                return false;
            }

            return true;
        }


        private bool RequestAllChannels(Message msg)
        {
            if (msg.Header == Headers.GET_ALL_CHANNEL)
            {
                OnReceiveChannelsInfo?.Invoke(msg.Body.FromJson<RequestChannelsInfoBody>(), this);

                return false;
            }

            return true;
        }

        private bool ChangeChannel(Message msg)
        {
            if (msg.Header == Headers.CHANGE_CHANNEL)
            {
                _channel = msg.Channel;

                OnChannelChanged?.Invoke(msg, this);

                return false;
            }

            return true;
        }

        private bool SetChannel(Message msg)
        {
            if (msg.Header == Headers.SET_CHANNEL)
            {
                _channel = msg.Channel;

                return false;
            }

            return true;
        }

        private bool CheckHandShake(Message msg)
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
