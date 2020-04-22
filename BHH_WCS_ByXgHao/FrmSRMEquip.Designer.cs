namespace BHH_WCS_ByXgHao
{
    partial class FrmSRMEquip
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
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.rtb_conn = new System.Windows.Forms.RichTextBox();
            this.rtb_msg = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.rtb_conn);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.rtb_msg);
            this.splitContainer3.Size = new System.Drawing.Size(1036, 700);
            this.splitContainer3.SplitterDistance = 533;
            this.splitContainer3.TabIndex = 1;
            // 
            // rtb_conn
            // 
            this.rtb_conn.BackColor = System.Drawing.Color.Silver;
            this.rtb_conn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_conn.Location = new System.Drawing.Point(0, 0);
            this.rtb_conn.Name = "rtb_conn";
            this.rtb_conn.Size = new System.Drawing.Size(533, 700);
            this.rtb_conn.TabIndex = 0;
            this.rtb_conn.Text = "";
            // 
            // rtb_msg
            // 
            this.rtb_msg.BackColor = System.Drawing.Color.Silver;
            this.rtb_msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_msg.Location = new System.Drawing.Point(0, 0);
            this.rtb_msg.Name = "rtb_msg";
            this.rtb_msg.Size = new System.Drawing.Size(499, 700);
            this.rtb_msg.TabIndex = 1;
            this.rtb_msg.Text = "";
            // 
            // FrmSRMEquip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1036, 700);
            this.Controls.Add(this.splitContainer3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmSRMEquip";
            this.Text = "堆垛机";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmSRMEquip_Load);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.RichTextBox rtb_conn;
        private System.Windows.Forms.RichTextBox rtb_msg;

    }
}