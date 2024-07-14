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
    public partial class MessageControl : UserControl
    {
        public MessageControl(string user, string date, string message, Color fontColor, Color backColor)
        {
            InitializeComponent();
            this.BackColor = backColor;
            this.txtMessage.BackColor = backColor;
            this.txtMessage.ForeColor = fontColor;
            this.lblUser.Text = user;
            this.lblHour.Text = " on " + date;
            this.txtMessage.Text = message;
            this.lblHour.Location = new Point(lblUser.Left + lblUser.Width, lblHour.Location.Y);
            this.Height = 30 + txtMessage.Lines.Count() * 40;
        }

       
    }
}
