namespace BHH_WCS_ByXgHao
{
    partial class FrmAutoTransport
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rtb_Con = new System.Windows.Forms.RichTextBox();
            this.rtb_Task = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rtb_Con);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rtb_Task);
            this.splitContainer1.Size = new System.Drawing.Size(625, 304);
            this.splitContainer1.SplitterDistance = 308;
            this.splitContainer1.TabIndex = 0;
            // 
            // rtb_Con
            // 
            this.rtb_Con.BackColor = System.Drawing.Color.Silver;
            this.rtb_Con.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Con.Location = new System.Drawing.Point(0, 0);
            this.rtb_Con.Name = "rtb_Con";
            this.rtb_Con.Size = new System.Drawing.Size(308, 304);
            this.rtb_Con.TabIndex = 0;
            this.rtb_Con.Text = "";
            // 
            // rtb_Task
            // 
            this.rtb_Task.BackColor = System.Drawing.Color.Silver;
            this.rtb_Task.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_Task.Location = new System.Drawing.Point(0, 0);
            this.rtb_Task.Name = "rtb_Task";
            this.rtb_Task.Size = new System.Drawing.Size(313, 304);
            this.rtb_Task.TabIndex = 1;
            this.rtb_Task.Text = "";
            // 
            // FrmAutoTransport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 304);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmAutoTransport";
            this.Text = "箱式线";
            this.Load += new System.EventHandler(this.FrmAutoTransport_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox rtb_Con;
        private System.Windows.Forms.RichTextBox rtb_Task;
    }
}