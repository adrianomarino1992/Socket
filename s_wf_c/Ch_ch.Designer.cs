namespace s_wf_c
{
    partial class Ch_ch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtCreate = new System.Windows.Forms.TextBox();
            this.btnJoin = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lstChannels = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // txtCreate
            // 
            this.txtCreate.Font = new System.Drawing.Font("Bahnschrift SemiLight Condensed", 13.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtCreate.Location = new System.Drawing.Point(10, 27);
            this.txtCreate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCreate.Name = "txtCreate";
            this.txtCreate.PlaceholderText = "channel name to create/join";
            this.txtCreate.Size = new System.Drawing.Size(475, 29);
            this.txtCreate.TabIndex = 1;
            // 
            // btnJoin
            // 
            this.btnJoin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnJoin.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnJoin.ForeColor = System.Drawing.Color.White;
            this.btnJoin.Location = new System.Drawing.Point(322, 361);
            this.btnJoin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(163, 39);
            this.btnJoin.TabIndex = 5;
            this.btnJoin.Text = "Join";
            this.btnJoin.UseVisualStyleBackColor = false;
            this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(153, 361);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(163, 39);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lstChannels
            // 
            this.lstChannels.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lstChannels.FullRowSelect = true;
            this.lstChannels.Location = new System.Drawing.Point(12, 72);
            this.lstChannels.MultiSelect = false;
            this.lstChannels.Name = "lstChannels";
            this.lstChannels.Size = new System.Drawing.Size(473, 282);
            this.lstChannels.TabIndex = 7;
            this.lstChannels.UseCompatibleStateImageBehavior = false;
            this.lstChannels.View = System.Windows.Forms.View.List;
            this.lstChannels.SelectedIndexChanged += new System.EventHandler(this.lstChannels_SelectedIndexChanged);
            // 
            // Ch_ch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 424);
            this.Controls.Add(this.lstChannels);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnJoin);
            this.Controls.Add(this.txtCreate);
            this.Font = new System.Drawing.Font("Bahnschrift SemiLight Condensed", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Ch_ch";
            this.Text = "Ch_ch";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TextBox txtCreate;
        private Button btnJoin;
        private Button btnCancel;
        private ListView lstChannels;
    }
}