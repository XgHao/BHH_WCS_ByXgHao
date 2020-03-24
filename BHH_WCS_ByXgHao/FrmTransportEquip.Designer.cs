namespace BHH_WCS_ByXgHao
{
    partial class FrmTransportEquip
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbinfo = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbinfo);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(814, 349);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // rbinfo
            // 
            this.rbinfo.BackColor = System.Drawing.Color.Silver;
            this.rbinfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rbinfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbinfo.Location = new System.Drawing.Point(3, 17);
            this.rbinfo.Name = "rbinfo";
            this.rbinfo.Size = new System.Drawing.Size(808, 329);
            this.rbinfo.TabIndex = 0;
            this.rbinfo.Text = "";
            // 
            // FrmTransportEquip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 349);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmTransportEquip";
            this.Text = "输送机";
            this.Load += new System.EventHandler(this.FrmTransportEquip_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rbinfo;
    }
}