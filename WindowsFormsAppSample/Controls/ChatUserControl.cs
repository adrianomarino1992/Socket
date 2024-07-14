using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsSample.Controls
{
    public partial class ChatUserControl : UserControl
    {
        public string User { get; }
        public ChatUserControl(string user)
        {
            InitializeComponent();
            this.User = user;
            this.Typing(false);
            this.lblUser.Text = user;

        }

        public void Typing(bool isTyping)
        {
            lblStatus.Visible = isTyping;
            this.Height = lblUser.Height + 3 + (isTyping ? lblStatus.Height + 3 : 0);
        }

    }
}
