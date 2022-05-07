using Socket.Client;
using Socket.DTO;
using System.ComponentModel;


namespace s_wf_c
{
    public partial class Ch_c : Form
    {
        private SocketC _socket;
        public Ch_c(SocketC socket)
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

            _socket.OnChannelChanged += _socket_OnChannelChanged;
            _socket.OnHandShakeDone += _socket_OnHandShakeDone;
            _socket.OnMessageReceived += _socket_OnMessageReceived;
            _socket.OnNewSocketEnterInTheChannel += _socket_OnNewSocketEnterInTheChannel;
            _socket.OnSocketLeftInTheChannel += _socket_OnSocketLeftInTheChannel;
            _socket.OnConnectionCheckedAsync += _socket_OnConnectionCheckedAsync;
            _socket.OnPartsOfChannelUpdated += _socket_OnPartsOfChannelUpdated;
            _socket.OnReceiveChannelsInfo += _socket_OnReceiveChannelsInfo;
           

            _socket.RequestOthersPartsOfChannel();

        }

        private void _socket_OnReceiveChannelsInfo(Socket.Messages.Body.RequestChannelsInfoBody arg1, SocketC arg2)
        {

            Ch_ch lg = new Ch_ch(arg1);

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

        private void _socket_OnPartsOfChannelUpdated(List<UserDTO> arg1, string arg2, SocketC arg3)
        {
            m_onMainT(() =>
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


        private void _socket_OnSocketLeftInTheChannel(Socket.Messages.Message obj)
        {
            m_onMainT(() =>
            {
                lvlUser.Items.Clear();

                foreach (var p in obj.ChannelsParts)
                {
                    lvlUser.Items.Add($"{p.Name}#{p.GUID}");
                }
            });
        }

        private void _socket_OnNewSocketEnterInTheChannel(Socket.Messages.Message obj)
        {
            m_onMainT(() =>
            {
                lvlUser.Items.Clear();

                foreach (var p in obj.ChannelsParts)
                {
                    lvlUser.Items.Add($"{p.Name}#{p.GUID}");
                }
            });
        }

        private void _socket_OnMessageReceived(Socket.Messages.Message arg1, SocketC arg2)
        {
            m_onMainT(() =>
            {
                if(arg1.TGUID == lblUser.Text.Split('#')[1])
                {
                    lvMsg.Items.Add(new ListViewItem { Text = $"Private from {arg1.From}: {arg1.Body}", ToolTipText = $"Received in {DateTime.Now}", BackColor = Color.FromArgb(1, 240, 189, 189) }).Focused = true;

                }
                else {

                    lvMsg.Items.Add(new ListViewItem { Text = $"{arg1.From}: {arg1.Body}", ToolTipText = $"Received in {DateTime.Now}", BackColor = Color.FromArgb(1, 207, 255, 247) }).Focused = true;

                }



            });

        }

        private void _socket_OnHandShakeDone(Socket.Messages.Message arg1, SocketC arg2)
        {
            m_onMainT(() =>
            {
                lvMsg.Items.Add($"HandShake bem sucedido");

                lvlUser.Items.Clear();

                foreach (var p in arg1.ChannelsParts)
                {
                    lvlUser.Items.Add($"{p.Name}#{p.GUID}");
                }
            });
        }

        private void _socket_OnConnectionCheckedAsync(bool arg1, DateTime arg2)
        {
            m_onMainT(() =>
            {
                if (arg1)
                {
                    lblStatus.Text = $"Ok : {arg2.ToString()}";
                    lblStatus.ForeColor = Color.DarkOliveGreen;
                }
                else
                {
                    lblStatus.Text = $"Off : {arg2.ToString()}";
                    lblStatus.ForeColor = Color.Red;
                }
            });

        }

        private void _socket_OnChannelChanged(Socket.Messages.Message arg1, SocketC arg2)
        {
            m_onMainT(() =>
            {
                lvMsg.Items.Clear();
                lvMsg.Items.Add(new ListViewItem { Text = $"Changed to channel: {arg1.Channel}", ToolTipText = $"Changed in {DateTime.Now}", BackColor = Color.FromArgb(1, 240, 222, 189) }).Focused = true;
                
                lblChannel.Text = arg1.Channel;
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtMsg.Text.Trim()))
                return;

            string uid = m_getUid();
            if(String.IsNullOrEmpty(uid) && uid != lblUser.Text.Split('#')[1])
            {
                lvMsg.Items.Add(new ListViewItem { Text = $"You: {txtMsg.Text.Trim()}", ToolTipText = $"Sent in {DateTime.Now}", BackColor = Color.FromArgb(1, 255, 239, 196 ) }).Focused = true;
                _socket.SendMessage(txtMsg.Text);
            }
            else
            {
                lvMsg.Items.Add(new ListViewItem { Text = $"Private to {m_getUser(uid)}: {txtMsg.Text.Trim()}", ToolTipText = $"Sent in {DateTime.Now}", BackColor = Color.FromArgb(1, 240, 189, 189)}).Focused = true;
                _socket.SendMessageTo(txtMsg.Text, uid);
            }

            

            txtMsg.Text = String.Empty;
        }

        private string m_getUser(string uid)
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

        private string m_getUid()
        {
            if(txtMsg.Text.Length > 6)
            {
                foreach(string u in lvlUser.Items)
                {
                    if(u.Split('#')[1] == txtMsg.Text.Substring(1, 5))
                    {
                        string uid = txtMsg.Text.Substring(1, 5);
                        txtMsg.Text = txtMsg.Text.Substring(6);
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

        private void m_onMainT(Action action)
        {
            this.Invoke(new Action(action));
        }

        private void Ch_c_Load(object sender, EventArgs e)
        {

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
                txtMsg.Text = $"#{lvlUser.SelectedItem.ToString().Split('#')[1]}:";
            }
            catch { }
        }
    }
}