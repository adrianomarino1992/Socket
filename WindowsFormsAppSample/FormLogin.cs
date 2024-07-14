using MySocket.Client;

namespace WinFormsSample
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
            _socket.OnConnected += OnConnectedHandle;
            _socket.OnHandShakeDone += OnHandShakeDoneHandle;
            _socket.OnConnectionFail += OnConnectionFailHandle;
            
            _socket.Connect(host.Trim(), port);
        }

        

        private void OnHandShakeDoneHandle(MySocket.Messages.Message arg1, SocketClient arg2)
        {
            this.Hide();

            _socket.OnConnected -= OnConnectedHandle;
            _socket.OnHandShakeDone -= OnHandShakeDoneHandle;
            _socket.OnConnectionFail -= OnConnectionFailHandle;

            _ = Task.Run(async () => {

                await Task.Delay(500);

                this.Invoke(new Action(() =>
                {
                    new FormChat(_socket).Show();

                }));
            
            });
        }

        private void OnConnectionFailHandle(Exception obj)
        {
            lblInfo.Text = "Fail on stablish a connection";
        }

        private void OnConnectedHandle(SocketClient obj)
        {
            lblInfo.Text = "Connected";
        }
    }
}
