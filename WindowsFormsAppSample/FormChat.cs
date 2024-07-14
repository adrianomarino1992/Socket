using MySocket.Client;
using MySocket.DTO;
using System.ComponentModel;


namespace s_wf_c
{
    public partial class FormChat : Form
    {
        private SocketClient _socket;
        public FormChat(SocketClient socket)
        {
            InitializeComponent();
            _socket = socket;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            lvMsg.Columns[0].Width = lvMsg.Width - 4;
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

            _socket.RequestOthersPartsOfChannel();

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

                lvlUser.Items.Clear();

                foreach (var p in arg1)
                {
                    lvlUser.Items.Add($"{p.Name}#{p.GUID}");
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
                lvlUser.Items.Clear();

                foreach (var p in users)
                {
                    lvlUser.Items.Add($"{p.Name}#{p.GUID}");
                }

                AddMessage($"{body.Name} left the room", $"Left in {DateTime.Now}", MsgTypes.USERLEFTTHERROM);

            });
        }

        private void OnNewSocketEnterInTheChannelHandle(MySocket.Messages.Body.UserEnterOrLeaveTheChannelBody body, List<UserDTO> users)
        {
            ExecuteOnMainThread(() =>
            {
                lvlUser.Items.Clear();

                foreach (var p in users)
                {
                    lvlUser.Items.Add($"{p.Name}#{p.GUID}");
                }

                AddMessage($"{body.Name} joined in the room", $"Joined in {DateTime.Now}", MsgTypes.USERENTEREDTHEROOM);
            });
        }

        private void OnMessageReceivedHandle(MySocket.Messages.Message arg1, SocketClient arg2)
        {
            ExecuteOnMainThread(() =>
            {
                if(arg1.TGUID == lblUser.Text.Split('#')[1])
                {
                    AddMessage($"Private from {arg1.From}: {arg1.Body}", $"Received in {DateTime.Now}", MsgTypes.PRIVATE);
                }
                else {

                    AddMessage($"{arg1.From}: {arg1.Body}", $"Received in {DateTime.Now}", MsgTypes.FROMOTHERUSER);
                }



            });

        }

        private void OnHandShakeDoneHandle(MySocket.Messages.Message arg1, SocketClient arg2)
        {
            ExecuteOnMainThread(() =>
            {                

                lvlUser.Items.Clear();

                foreach (var p in arg1.ChannelsParts)
                {
                    lvlUser.Items.Add($"{p.Name}#{p.GUID}");
                }
            });
        }

        private void OnConnectionCheckedAsyncHandle(bool arg1, DateTime arg2)
        {
            ExecuteOnMainThread(() =>
            {
                if (arg1)
                {
                    lblStatus.Text = $"Ok : {arg2.ToString()}";
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
                lvMsg.Items.Clear();
                AddMessage($"Changed to channel: {arg1.Channel}", $"Changed in {DateTime.Now}", MsgTypes.CHANGECHANNEL);
                
                lblChannel.Text = arg1.Channel;
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtMsg.Text.Trim()))
                return;

            string uid = GetUID();
            if(String.IsNullOrEmpty(uid) && uid != lblUser.Text.Split('#')[1])
            {
                AddMessage($"You: {txtMsg.Text.Trim()}", $"Sent in {DateTime.Now}", MsgTypes.FROMTHISUSER);
                _socket.SendMessage(txtMsg.Text);
            }
            else
            {
                AddMessage($"Private to {GetUserFromGUID(uid)}: {txtMsg.Text.Trim()}", $"Sent in {DateTime.Now}", MsgTypes.PRIVATE);
                _socket.SendMessageTo(txtMsg.Text, uid);
            }

            

            txtMsg.Text = String.Empty;
        }

        private string GetUserFromGUID(string uid)
        {
            foreach (string u in lvlUser.Items)
            {
                if (u.Split('#')[1] ==uid)
                {
                    return u.Split('#')[0];
                }
            }
            return String.Empty;
        }

        private string GetUID()
        {
            if(txtMsg.Text.Length > 8)
            {
                foreach(string u in lvlUser.Items)
                {
                    if(u.Split('#')[1] == txtMsg.Text.Substring(1, 5))
                    {
                        string uid = txtMsg.Text.Substring(1, 5);
                        txtMsg.Text = txtMsg.Text.Substring(7);
                        return uid;
                    }
                }
            }
            return String.Empty;
        }
        private void txtMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnSend.PerformClick();
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

        private void lvlUser_Click(object sender, EventArgs e)
        {
            try
            {
                txtMsg.Text = $"#{lvlUser.SelectedItem?.ToString()?.Split('#')[1]}:";
            }
            catch { }
        }

        private void AddMessage(string msg, string tooltip, MsgTypes type)
        {
            Color colorB = Color.White;
            Color colorF = Color.Black;
            switch (type)
            {
                case MsgTypes.DEFAULT: { 
                        colorB = Color.FromArgb(1, 250, 242, 225); 
                        colorF = Color.FromArgb(1, 41, 41, 40); 
                    } break;
                case MsgTypes.PRIVATE:
                    {
                        colorB = Color.FromArgb(1, 130, 15, 3);
                        colorF = Color.FromArgb(1, 247, 247, 245);
                    }
                    break;
                case MsgTypes.FROMTHISUSER:
                    {
                        colorB = Color.FromArgb(1, 232, 255, 252);
                        colorF = Color.FromArgb(1, 41, 41, 40);
                    }
                    break;
                case MsgTypes.FROMOTHERUSER:
                    {
                        colorB = Color.FromArgb(1, 250, 242, 225);
                        colorF = Color.FromArgb(1, 41, 41, 40);
                    }
                    break;
                case MsgTypes.CHANGECHANNEL:
                    {
                        colorB = Color.FromArgb(1, 61, 61, 61);
                        colorF = Color.FromArgb(1, 250, 250, 250);
                    }
                    break;
                case MsgTypes.USERENTEREDTHEROOM:
                    {
                        colorB = Color.FromArgb(1, 245, 244, 213);
                        colorF = Color.FromArgb(1, 41, 41, 40);
                    }
                    break;
                case MsgTypes.USERLEFTTHERROM:
                    {
                        colorB = Color.FromArgb(1, 245, 244, 213);
                        colorF = Color.FromArgb(1, 41, 41, 40);
                    }
                    break;
            }

            lvMsg.Items.Add(new ListViewItem { Text = $"{DateTime.Now.ToShortTimeString()} - " +  msg, BackColor = colorB, ForeColor = colorF, ToolTipText = tooltip }).Focused = true;

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
       
    }
}