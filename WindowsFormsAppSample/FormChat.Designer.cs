namespace WinFormsSample
{
    partial class FormChat
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChat));
            this.lvlUser = new System.Windows.Forms.ListBox();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblChannel = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnRequestChannels = new System.Windows.Forms.PictureBox();
            this.flowPanelMessages = new System.Windows.Forms.FlowLayoutPanel();
            this.lblUser = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.btnRequestChannels)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvlUser
            // 
            this.lvlUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvlUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.lvlUser.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvlUser.FormattingEnabled = true;
            this.lvlUser.ItemHeight = 18;
            this.lvlUser.Location = new System.Drawing.Point(726, 53);
            this.lvlUser.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lvlUser.Name = "lvlUser";
            this.lvlUser.Size = new System.Drawing.Size(163, 378);
            this.lvlUser.TabIndex = 1;
            this.lvlUser.Click += new System.EventHandler(this.lvlUser_Click);
            // 
            // txtMsg
            // 
            this.txtMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMsg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMsg.Location = new System.Drawing.Point(16, 437);
            this.txtMsg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.PlaceholderText = "...";
            this.txtMsg.Size = new System.Drawing.Size(704, 51);
            this.txtMsg.TabIndex = 2;
            this.txtMsg.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMsg_KeyDown);
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Location = new System.Drawing.Point(726, 439);
            this.btnSend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(163, 50);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblChannel
            // 
            this.lblChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblChannel.AutoSize = true;
            this.lblChannel.Location = new System.Drawing.Point(726, 19);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(47, 18);
            this.lblChannel.TabIndex = 5;
            this.lblChannel.Text = "Channel";
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Bahnschrift SemiLight Condensed", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.ForeColor = System.Drawing.Color.DarkGray;
            this.lblStatus.Location = new System.Drawing.Point(10, 494);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(103, 16);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Checking connection ...";
            // 
            // btnRequestChannels
            // 
            this.btnRequestChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRequestChannels.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnRequestChannels.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRequestChannels.Image = ((System.Drawing.Image)(resources.GetObject("btnRequestChannels.Image")));
            this.btnRequestChannels.Location = new System.Drawing.Point(865, 17);
            this.btnRequestChannels.Name = "btnRequestChannels";
            this.btnRequestChannels.Padding = new System.Windows.Forms.Padding(4);
            this.btnRequestChannels.Size = new System.Drawing.Size(22, 22);
            this.btnRequestChannels.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnRequestChannels.TabIndex = 7;
            this.btnRequestChannels.TabStop = false;
            this.btnRequestChannels.Click += new System.EventHandler(this.btnRequestChannels_Click);
            // 
            // flowPanelMessages
            // 
            this.flowPanelMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanelMessages.AutoScroll = true;
            this.flowPanelMessages.BackColor = System.Drawing.Color.White;
            this.flowPanelMessages.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanelMessages.Location = new System.Drawing.Point(18, 48);
            this.flowPanelMessages.Name = "flowPanelMessages";
            this.flowPanelMessages.Size = new System.Drawing.Size(702, 383);
            this.flowPanelMessages.TabIndex = 8;
            this.flowPanelMessages.WrapContents = false;
            this.flowPanelMessages.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.flowPanelMessages_ControlAdded);
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(18, 17);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(87, 18);
            this.lblUser.TabIndex = 4;
            this.lblUser.Text = "UserName#Guid";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblUser);
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.flowPanelMessages);
            this.panel1.Controls.Add(this.lvlUser);
            this.panel1.Controls.Add(this.btnRequestChannels);
            this.panel1.Controls.Add(this.txtMsg);
            this.panel1.Controls.Add(this.btnSend);
            this.panel1.Controls.Add(this.lblChannel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(10, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(901, 521);
            this.panel1.TabIndex = 9;
            // 
            // FormChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(921, 541);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Bahnschrift SemiLight Condensed", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(937, 580);
            this.Name = "FormChat";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Chat";
            ((System.ComponentModel.ISupportInitialize)(this.btnRequestChannels)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private ListBox lvlUser;
        private TextBox txtMsg;
        private Button btnSend;
        private Label lblChannel;
        private Label lblStatus;
        private PictureBox btnRequestChannels;
        private FlowLayoutPanel flowPanelMessages;
        private Label lblUser;
        private Panel panel1;
    }
}