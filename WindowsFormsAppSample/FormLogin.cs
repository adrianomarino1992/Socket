using MySocket.Client;

namespace s_wf_c
{    
    public partial class FormLogin : Form
    {
        private SocketClient _socket;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            
            string host, user, ports = String.Empty;

            host = txtHost.Text;
            if(String.IsNullOrEmpty(txtHost.Text))
            {
                host = txtHost.PlaceholderText;
                
            }

            ports = txtPort.Text;
            if (String.IsNullOrEmpty(txtPort.Text) || !int.TryParse(txtPort.Text, out _))
            {
                ports = txtPort.PlaceholderText;
               
            }

            user = txtUser.Text;
            if (String.IsNullOrEmpty(txtUser.Text))
            {
                user = txtUser.PlaceholderText;
                
            }

            int port = int.Parse(ports);

            _socket = new SocketClient(user);
            _socket.OnConnected += _socket_OnConnected;
            _socket.OnHandShakeDone += _socket_OnHandShakeDone;
            _socket.OnConnectionFail += _socket_OnConnectionFail;
            
            _socket.Connect(host.Trim(), port);
        }

        

        private void _socket_OnHandShakeDone(MySocket.Messages.Message arg1, SocketClient arg2)
        {
            this.Hide();

            _socket.OnConnected -= _socket_OnConnected;
            _socket.OnHandShakeDone -= _socket_OnHandShakeDone;
            _socket.OnConnectionFail -= _socket_OnConnectionFail;

            _ = Task.Run(async () => {

                await Task.Delay(500);

                this.Invoke(new Action(() =>
                {
                    new FormChat(_socket).Show();

                }));
            
            });
        }

        private void _socket_OnConnectionFail(Exception obj)
        {
            lblInfo.Text = "Fail on stablish a connection";
        }

        private void _socket_OnConnected(SocketClient obj)
        {
            lblInfo.Text = "Connected";
        }
    }
}
