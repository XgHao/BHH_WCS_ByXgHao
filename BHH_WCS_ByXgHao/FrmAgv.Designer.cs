namespace BHH_WCS_ByXgHao
{
    partial class FrmAgv
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
            this.rb_SendAgvMsg = new System.Windows.Forms.RichTextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.gb_SendLog = new System.Windows.Forms.GroupBox();
            this.gb_RecvLog = new System.Windows.Forms.GroupBox();
            this.rb_RcvAgvMsg = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.gb_SendLog.SuspendLayout();
            this.gb_RecvLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // rb_SendAgvMsg
            // 
            this.rb_SendAgvMsg.BackColor = System.Drawing.Color.Silver;
            this.rb_SendAgvMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rb_SendAgvMsg.Location = new System.Drawing.Point(3, 17);
            this.rb_SendAgvMsg.Name = "rb_SendAgvMsg";
            this.rb_SendAgvMsg.Size = new System.Drawing.Size(354, 305);
            this.rb_SendAgvMsg.TabIndex = 0;
            this.rb_SendAgvMsg.Text = "";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.gb_SendLog);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.gb_RecvLog);
            this.splitContainer2.Size = new System.Drawing.Size(730, 325);
            this.splitContainer2.SplitterDistance = 360;
            this.splitContainer2.TabIndex = 3;
            // 
            // gb_SendLog
            // 
            this.gb_SendLog.Controls.Add(this.rb_SendAgvMsg);
            this.gb_SendLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_SendLog.Location = new System.Drawing.Point(0, 0);
            this.gb_SendLog.Name = "gb_SendLog";
            this.gb_SendLog.Size = new System.Drawing.Size(360, 325);
            this.gb_SendLog.TabIndex = 1;
            this.gb_SendLog.TabStop = false;
            this.gb_SendLog.Text = "发送给AGV日志";
            // 
            // gb_RecvLog
            // 
            this.gb_RecvLog.Controls.Add(this.rb_RcvAgvMsg);
            this.gb_RecvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_RecvLog.Location = new System.Drawing.Point(0, 0);
            this.gb_RecvLog.Name = "gb_RecvLog";
            this.gb_RecvLog.Size = new System.Drawing.Size(366, 325);
            this.gb_RecvLog.TabIndex = 0;
            this.gb_RecvLog.TabStop = false;
            this.gb_RecvLog.Text = "接收AGV日志";
            // 
            // rb_RcvAgvMsg
            // 
            this.rb_RcvAgvMsg.BackColor = System.Drawing.Color.Silver;
            this.rb_RcvAgvMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rb_RcvAgvMsg.Location = new System.Drawing.Point(3, 17);
            this.rb_RcvAgvMsg.Name = "rb_RcvAgvMsg";
            this.rb_RcvAgvMsg.Size = new System.Drawing.Size(360, 305);
            this.rb_RcvAgvMsg.TabIndex = 0;
            this.rb_RcvAgvMsg.Text = "";
            // 
            // FrmAgv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 325);
            this.Controls.Add(this.splitContainer2);
            this.Name = "FrmAgv";
            this.Text = "AGV日志";
            this.Load += new System.EventHandler(this.FrmAgv_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.gb_SendLog.ResumeLayout(false);
            this.gb_RecvLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rb_SendAgvMsg;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox gb_SendLog;
        private System.Windows.Forms.GroupBox gb_RecvLog;
        private System.Windows.Forms.RichTextBox rb_RcvAgvMsg;
    }
}