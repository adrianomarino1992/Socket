using MySocket.Client;
using MySocket.DTO;
using MySocket.Messages;
using MySocket.Messages.Body;

using System.Net;
using System.Net.Sockets;

namespace MySocket.Server
{
    public class SocketServer
    {
        #region EVENTS

        public event Action<SocketClient> OnClientAccepted;
        public event Action<SocketClient> OnClientDisconnect;
        public event Action<MySocket.Server.SocketServer> OnServerUp;
        public event Action<MySocket.Server.SocketServer> OnServerDown;
        public event Action<MySocket.Server.SocketServer> OnTryUpServerFail;
        public event Action<Message, SocketClient> OnMessageReceived;
        public event Action<Exception> OnExceptionReceived;
        public event Action<ChangeChannelBody, SocketClient> OnClientChangeChannel;
        #endregion


        #region PRIVATE FIELDS
        private TcpListener _tcpListener;
        private Thread _thread;
        private bool _keepRunning;
        private bool _isAlive { get => _thread != null && _thread.IsAlive; }
        #endregion


        #region PUBLIC PROPERTIES
        public string DefaultChannel { get => Configurations.Config.ServerName; }
        public bool Connected => IsConnected();

        #endregion

#pragma warning disable
        public SocketServer(IPEndPoint localEPt)
        {
#pragma warning enable
            _tcpListener = new TcpListener(localEPt);
        }

        #region CONNECTION METHODS
        public void Connect()
        {
            if (IsConnected())
            {
                OnServerUp?.Invoke(this);
                return;
            }

            Start();
        }

        public void Disconnect()
        {
            try
            {
                if (!IsConnected())
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
        public void SendMessage(string codMsg, SocketClient socket = null)
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
        private void Start()
        {
            try
            {
                _tcpListener.Start();

                _keepRunning = true;

                MySocket.Channel.Channel.CreateChannel(DefaultChannel, this);

                _thread = new Thread(BackGroundWork);

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

        private bool IsConnected()
        {
           return _tcpListener.Server.IsBound;
        }
        private void BackGroundWork()
        {
            while (_isAlive && _keepRunning)
            {
                try
                {
                    SocketClient incomming = new SocketClient(_tcpListener.AcceptTcpClient());

                    incomming.OnMessageArriveFromClient += MessageArriveFromClient;
                    incomming.OnHandShakeDone += HandShakeDone;
                }
                catch (Exception ex)
                {
                    OnExceptionReceived?.Invoke(ex);
                }
            }

        }

        private void HandShakeDone(Message msg, SocketClient socket)
        {
            try
            {
                if (!CheckGUID(msg, socket))
                    return;
            }
            catch (Exception ex)
            {
                OnExceptionReceived?.Invoke(ex);
            }
        }


        private void MessageArriveFromClient(Message codMsg, SocketClient socketC)
        {
            try
            {
                if (!HandleInternalMessage(codMsg, socketC))
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

        private bool HandleInternalMessage(Message codMsg, SocketClient socketC)
        {
            if (codMsg.Header == Headers.DISCONNECT)
            {
                DisconnectClient(socketC);

                return false;
            }

            if (codMsg.Header == Headers.CHANGE_CHANNEL)
            {

                ChangeClientChannel(codMsg, socketC);

                return false;
            }

            if (codMsg.Header == Headers.GET_PARTS_CHANNEL)
            {

                SendChannelParts(codMsg, socketC);

                return false;
            }

            if (codMsg.Header == Headers.GET_ALL_CHANNEL)
            {

                SendChannelsInfoMessage(socketC);

                return false;
            }

            if (codMsg.Header == Headers.CUSTOM_EVENT)
            {

                EmitEventOnClient(codMsg, socketC);

                return false;
            }


            return true;
        }


        private void EmitEventOnClient(Message msg, SocketClient socket)
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


        private void SendChannelsInfoMessage(SocketClient socket)
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

        private void SendChannelParts(Message chgN, SocketClient socket)
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
        private void ChangeClientChannel(Message chgN, SocketClient socket)
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

        private Message CreateMessage()
        {
            return new Message
            {
                DateTime = DateTime.Now,
                From = Configurations.Config.ServerName,
                FGUID = Configurations.Config.ServerGUID
            };
        }

        private void DisconnectClient(SocketClient socketC)
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

        private bool CheckGUID(Message message, SocketClient socketC)
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
