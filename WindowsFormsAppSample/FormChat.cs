using MySocket.Client;
using MySocket.DTO;
using System.ComponentModel;
using global::WinFormsSample.Controls;


namespace WinFormsSample
{
    public partial class FormChat : Form
    {
        private DateTime _lastKeyPressed = DateTime.MinValue;
        private bool _isTyping = false;

        private SocketClient _socket;
        public FormChat(SocketClient socket)
        {
            InitializeComponent();
            _socket = socket;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            lblChannel.Text = _socket.Channel;
            lblUser.Text = $"{_socket.UserName}#{_socket.GUID}";

            _socket.OnChannelChanged += OnChannelChangedHandle;
            _socket.OnHandShakeDone += OnHandShakeDoneHandle;
            _socket.OnMessageReceived += OnMessageReceivedHandle;
            _socket.OnNewSocketEnterInTheChannel += OnNewSocketEnterInTheChannelHandle;
            _socket.OnSocketLeftInTheChannel += OnSocketLeftInTheChannelHandle;
            _socket.OnConnectionCheckedAsync += OnConnectionCheckedAsyncHandle;
            _socket.OnPartsOfChannelUpdated += OnPartsOfChannelUpdatedHandle;
            _socket.OnReceiveChannelsInfo += OnReceiveChannelsInfoHandle;
            _socket.OnReconnected += OnReconnectedHandle;
            _socket.OnReconnectFail += OnReconnectFailHandle;

            _socket.On("AlguemDigitou", msg =>
            {
                ExecuteOnMainThread(() =>
                {
                    ChatUserControl? user = null;
                    foreach (var control in flowPanelUsers.Controls)
                    {
                        user = control! as ChatUserControl;

                        if (user!.User == msg.From + "#" + msg.FGUID)
                        {
                            user.Typing(true);
                            return;
                        }
                    }

                });

            });

            _socket.On("AlguemParouDeDigitou", msg =>
            {
                ExecuteOnMainThread(() =>
                {
                    ChatUserControl? user = null;
                    foreach (var control in flowPanelUsers.Controls)
                    {
                        user = control! as ChatUserControl;

                        if (user!.User == msg.From + "#" + msg.FGUID)
                        {
                            user.Typing(false);
                            return;
                        }
                    }    
                    
                });

            });

            _socket.RequestOthersPartsOfChannel();

            this.lblStatus.Location = new Point(lblUser.Left + lblUser.Width, lblStatus.Location.Y);

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000);

                    if (_isTyping && _lastKeyPressed.AddSeconds(5) < DateTime.Now)
                    {
                        _isTyping = false;
                        _socket.Emit("AlguemParouDeDigitou", "", true);
                    }
                }

            });

        }

        private void OnReconnectFailHandle(SocketClient obj)
        {
            ExecuteOnMainThread(() =>
            {
                AddMessage("Fail on reconnect to the server", $"Last try in {DateTime.Now}", MsgTypes.CHANGECHANNEL);
                lblStatus.Text = $"Fail on reconnect to the server: {DateTime.Now}";
                lblStatus.ForeColor = Color.Red;
            });
        }

        private void OnReconnectedHandle(SocketClient obj)
        {
            AddMessage("Reconnected to the server", $"Reconnected in {DateTime.Now}", MsgTypes.CHANGECHANNEL);
        }

        private void OnReceiveChannelsInfoHandle(MySocket.Messages.Body.RequestChannelsInfoBody arg1, SocketClient arg2)
        {

            FormSelectChannel lg = new FormSelectChannel(arg1);

            if (lg.ShowDialog() == DialogResult.OK)
            {
                _socket.ChangeChannel(lg.Channel);

                _ = Task.Run(async () =>
                {
                    await Task.Delay(300);
                    _socket.RequestOthersPartsOfChannel();

                });

            }
        }

        private void OnPartsOfChannelUpdatedHandle(List<UserDTO> arg1, string arg2, SocketClient arg3)
        {
            ExecuteOnMainThread(() =>
            {
                lblChannel.Text = arg2;

                flowPanelUsers.Controls.Clear();

                foreach (var p in arg1.OrderBy(s => s.Name))
                {
                    flowPanelUsers.Controls.Add(new ChatUserControl(p.Name + "#" + p.GUID));
                }
            });

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _socket.Disconnect();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }


        private void OnSocketLeftInTheChannelHandle(MySocket.Messages.Body.UserEnterOrLeaveTheChannelBody body, List<UserDTO> users)
        {
            ExecuteOnMainThread(() =>
            {

                ChatUserControl? userToRemove = null;
                foreach (var control in flowPanelUsers.Controls)
                {
                    userToRemove = control! as ChatUserControl;
                    break;
                }

                if (userToRemove is null)
                    return;

                flowPanelUsers.Controls.Remove(userToRemove);

                AddMessage("Server", $"{body.Name} left the room", MsgTypes.USERLEFTTHERROM);

            });
        }

        private void OnNewSocketEnterInTheChannelHandle(MySocket.Messages.Body.UserEnterOrLeaveTheChannelBody body, List<UserDTO> users)
        {
            ExecuteOnMainThread(() =>
            {
                flowPanelUsers.Controls.Clear();

                foreach (var p in users.OrderBy(s => s.Name))
                {
                    flowPanelUsers.Controls.Add(new ChatUserControl(p.Name + "#" + p.GUID));
                }

                AddMessage("Server",  $"{body.Name} joined in the room", MsgTypes.USERENTEREDTHEROOM);
            });
        }

        private void OnMessageReceivedHandle(MySocket.Messages.Message arg1, SocketClient arg2)
        {
            ExecuteOnMainThread(() =>
            {
                if(arg1.TGUID == lblUser.Text.Split('#')[1])
                {
                    AddMessage(arg1.From, arg1.Body, MsgTypes.PRIVATE);
                }
                else {

                    AddMessage(arg1.From, arg1.Body, MsgTypes.FROMOTHERUSER);
                }



            });

        }

        private void OnHandShakeDoneHandle(MySocket.Messages.Message arg1, SocketClient arg2)
        {
            ExecuteOnMainThread(() =>
            {

                flowPanelUsers.Controls.Clear();

                foreach (var p in arg1.ChannelsParts.OrderBy(s => s.Name))
                {
                    flowPanelUsers.Controls.Add(new ChatUserControl(p.Name + "#" + p.GUID));
                }
            });
        }

        private void OnConnectionCheckedAsyncHandle(bool arg1, DateTime arg2)
        {
            ExecuteOnMainThread(() =>
            {
                if (arg1)
                {
                    lblStatus.Text = $"Connected";
                    lblStatus.ForeColor = Color.DarkOliveGreen;
                }
                else
                {
                    lblStatus.Text = $"Trying to reconnect : {arg2.ToString()}";
                    lblStatus.ForeColor = Color.Red;
                    _socket.Reconnect(10);
                }
            });

        }

        private void OnChannelChangedHandle(MySocket.Messages.Message arg1, SocketClient arg2)
        {
            ExecuteOnMainThread(() =>
            {
                flowPanelMessages.Controls.Clear();
                AddMessage($"Changed to channel: {arg1.Channel}", $"Changed in {DateTime.Now}", MsgTypes.CHANGECHANNEL);
                
                lblChannel.Text = arg1.Channel;
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtMsg.Text.Trim()))
                return;

            _socket.Emit("AlguemParouDeDigitou", "", true);
            _isTyping = false;


            Task.Run(async () =>
            {
                await Task.Delay(500);

                this.Invoke(new Action(() =>
                {
                    AddMessage("You", txtMsg.Text.Trim(), MsgTypes.FROMTHISUSER);
                    _socket.SendMessage(txtMsg.Text);

                    txtMsg.Text = String.Empty;

                }));
            });

            
        }

       
        private void txtMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                btnSend_Click(null, null);                

            }
            else
            {
                _lastKeyPressed = DateTime.Now;

                if (!_isTyping)
                {
                    _socket.Emit("AlguemDigitou", "", true);
                    _isTyping = true;
                }
            }
        }

        private void ExecuteOnMainThread(Action action)
        {
            this.Invoke(new Action(action));
        }

       

        private void lblChannel_Click(object sender, EventArgs e)
        {

        }

        private void btnRequestChannels_Click(object sender, EventArgs e)
        {
            _socket.RequestAllChannels();
        }

        

        private void AddMessage(string user, string msg, MsgTypes type)
        {
            Color colorB = Color.White;
            Color colorF = Color.Black;
            switch (type)
            {
                case MsgTypes.DEFAULT: { 
                        colorB = Color.FromArgb(250, 242, 225); 
                        colorF = Color.FromArgb(41, 41, 40); 
                    } break;
                case MsgTypes.PRIVATE:
                    {
                        colorB = Color.FromArgb(130, 15, 3);
                        colorF = Color.FromArgb(247, 247, 245);
                    }
                    break;
                case MsgTypes.FROMTHISUSER:
                    {
                        colorB = Color.FromArgb(232, 255, 252);
                        colorF = Color.FromArgb(41, 41, 40);
                    }
                    break;
                case MsgTypes.FROMOTHERUSER:
                    {
                        colorB = Color.FromArgb(250, 242, 225);
                        colorF = Color.FromArgb(41, 41, 40);
                    }
                    break;
                case MsgTypes.CHANGECHANNEL:
                    {
                        colorB = Color.FromArgb(61, 61, 61);
                        colorF = Color.FromArgb(250, 250, 250);
                    }
                    break;
                case MsgTypes.USERENTEREDTHEROOM:
                    {
                        colorB = Color.FromArgb(245, 244, 213);
                        colorF = Color.FromArgb(41, 41, 40);
                    }
                    break;
                case MsgTypes.USERLEFTTHERROM:
                    {
                        colorB = Color.FromArgb(245, 244, 213);
                        colorF = Color.FromArgb(41, 41, 40);
                    }
                    break;
            }

            flowPanelMessages.Controls.Add(new MessageControl(user, DateTime.Now.ToShortTimeString(), msg, colorF, colorB) { Dock = DockStyle.Top});

            

        }

        private enum MsgTypes
        {
            DEFAULT,
            PRIVATE, 
            FROMTHISUSER,
            FROMOTHERUSER,
            CHANGECHANNEL,
            USERENTEREDTHEROOM,
            USERLEFTTHERROM
            
        }

        private void flowPanelMessages_ControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Width = flowPanelMessages.Width - 25;
            e.Control.Select();
        }
               


        private void flowPanelUsers_ControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Width = flowPanelUsers.Width - 25;
            e.Control.Margin = new Padding(0);
            
        }
    }
}