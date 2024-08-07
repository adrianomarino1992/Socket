﻿using MySocket.DTO;
using MySocket.Messages.Body;

namespace WinFormsSample
{
    public partial class FormSelectChannel : Form
    {
        private RequestChannelsInfoBody _info;

        public string Channel { get; set; }
        public FormSelectChannel(RequestChannelsInfoBody info)
        {
            InitializeComponent();

            _info = info;

        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            lstChannels.Items.Clear();

            foreach (ChannelDTO c in _info.Channels)
            {
                lstChannels.Items.Add(new ListViewItem() { Text = c.Name + $" ({c.Users.Count})", ToolTipText = c.Name });
            }
        }

        private void lstChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstChannels.SelectedItems.Count > 0)
                txtCreate.Text = lstChannels.SelectedItems[0].ToolTipText;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtCreate.Text.Trim()))
                return;

            this.DialogResult= DialogResult.OK;
            this.Channel = txtCreate.Text.Trim();
            this.Close();
        }
    }
}
