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

                lvMsg.Items.Add($"{arg1.From}: {arg1.Body}");

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
                lvMsg.Items.Add($"Changed to channel: {arg1.Channel}");
                lblChannel.Text = arg1.Channel;
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtMsg.Text.Trim()))
                return;

            lvMsg.Items.Add($"You: {txtMsg.Text.Trim()}");

            _socket.SendMessage(txtMsg.Text);

            txtMsg.Text = String.Empty;
        }

        private void txtMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
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
    }
}