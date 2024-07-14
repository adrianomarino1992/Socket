namespace s_wf_c
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
            this.lblUser = new System.Windows.Forms.Label();
            this.lblChannel = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnRequestChannels = new System.Windows.Forms.PictureBox();
            this.lvMsg = new System.Windows.Forms.ListView();
            this.colMsg = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.btnRequestChannels)).BeginInit();
            this.SuspendLayout();
            // 
            // lvlUser
            // 
            this.lvlUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvlUser.BackColor = System.Drawing.Color.White;
            this.lvlUser.FormattingEnabled = true;
            this.lvlUser.ItemHeight = 18;
            this.lvlUser.Location = new System.Drawing.Point(746, 50);
            this.lvlUser.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lvlUser.Name = "lvlUser";
            this.lvlUser.Size = new System.Drawing.Size(163, 400);
            this.lvlUser.TabIndex = 1;
            this.lvlUser.Click += new System.EventHandler(this.lvlUser_Click);
            // 
            // txtMsg
            // 
            this.txtMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMsg.BackColor = System.Drawing.Color.White;
            this.txtMsg.Location = new System.Drawing.Point(10, 461);
            this.txtMsg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.PlaceholderText = "...";
            this.txtMsg.Size = new System.Drawing.Size(730, 51);
            this.txtMsg.TabIndex = 2;
            this.txtMsg.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMsg_KeyDown);
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Location = new System.Drawing.Point(746, 463);
            this.btnSend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(163, 50);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(12, 18);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(87, 18);
            this.lblUser.TabIndex = 4;
            this.lblUser.Text = "UserName#Guid";
            // 
            // lblChannel
            // 
            this.lblChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblChannel.AutoSize = true;
            this.lblChannel.Location = new System.Drawing.Point(746, 18);
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
            this.lblStatus.Location = new System.Drawing.Point(12, 519);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(103, 16);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Checking connection ...";
            // 
            // btnRequestChannels
            // 
            this.btnRequestChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRequestChannels.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnRequestChannels.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRequestChannels.Image = ((System.Drawing.Image)(resources.GetObject("btnRequestChannels.Image")));
            this.btnRequestChannels.Location = new System.Drawing.Point(885, 16);
            this.btnRequestChannels.Name = "btnRequestChannels";
            this.btnRequestChannels.Padding = new System.Windows.Forms.Padding(4);
            this.btnRequestChannels.Size = new System.Drawing.Size(22, 22);
            this.btnRequestChannels.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnRequestChannels.TabIndex = 7;
            this.btnRequestChannels.TabStop = false;
            this.btnRequestChannels.Click += new System.EventHandler(this.btnRequestChannels_Click);
            // 
            // lvMsg
            // 
            this.lvMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvMsg.AutoArrange = false;
            this.lvMsg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colMsg});
            this.lvMsg.FullRowSelect = true;
            this.lvMsg.Location = new System.Drawing.Point(12, 50);
            this.lvMsg.MultiSelect = false;
            this.lvMsg.Name = "lvMsg";
            this.lvMsg.RightToLeftLayout = true;
            this.lvMsg.ShowItemToolTips = true;
            this.lvMsg.Size = new System.Drawing.Size(728, 400);
            this.lvMsg.TabIndex = 8;
            this.lvMsg.UseCompatibleStateImageBehavior = false;
            this.lvMsg.View = System.Windows.Forms.View.Details;
            // 
            // colMsg
            // 
            this.colMsg.Text = "Messages:";
            this.colMsg.Width = 500;
            // 
            // Ch_c
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(921, 541);
            this.Controls.Add(this.lvMsg);
            this.Controls.Add(this.btnRequestChannels);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblChannel);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.lvlUser);
            this.Font = new System.Drawing.Font("Bahnschrift SemiLight Condensed", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(937, 580);
            this.Name = "Ch_c";
            this.Text = "Chat";
            ((System.ComponentModel.ISupportInitialize)(this.btnRequestChannels)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ListBox lvlUser;
        private TextBox txtMsg;
        private Button btnSend;
        private Label lblUser;
        private Label lblChannel;
        private Label lblStatus;
        private PictureBox btnRequestChannels;
        private ListView lvMsg;
        private ColumnHeader colMsg;
    }
}