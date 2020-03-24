namespace BHH_WCS_ByXgHao
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSystemRenderer toolStripSystemRenderer2 = new System.Windows.Forms.ToolStripSystemRenderer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmi_System = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Restart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Calc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Notepad = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Helpword = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Support = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Version = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.notifyWCS = new System.Windows.Forms.NotifyIcon(this.components);
            this.NicontextMeun = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmi_Show = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Hide = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Quit = new System.Windows.Forms.ToolStripMenuItem();
            this.TabControl1 = new MdiTabControl.TabControl();
            this.menuStrip1.SuspendLayout();
            this.NicontextMeun.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_System,
            this.tsmi_Tools,
            this.tsmi_Help});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(791, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmi_System
            // 
            this.tsmi_System.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Restart,
            this.tsmi_Exit});
            this.tsmi_System.Name = "tsmi_System";
            this.tsmi_System.Size = new System.Drawing.Size(59, 21);
            this.tsmi_System.Text = "系统(&S)";
            // 
            // tsmi_Restart
            // 
            this.tsmi_Restart.Name = "tsmi_Restart";
            this.tsmi_Restart.Size = new System.Drawing.Size(180, 22);
            this.tsmi_Restart.Text = "重启(&R)";
            this.tsmi_Restart.Click += new System.EventHandler(this.Tsmi_Restart_Click);
            // 
            // tsmi_Exit
            // 
            this.tsmi_Exit.Name = "tsmi_Exit";
            this.tsmi_Exit.Size = new System.Drawing.Size(180, 22);
            this.tsmi_Exit.Text = "退出系统(&E)";
            this.tsmi_Exit.Click += new System.EventHandler(this.Tsmi_Exit_Click);
            // 
            // tsmi_Tools
            // 
            this.tsmi_Tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Calc,
            this.tsmi_Notepad});
            this.tsmi_Tools.Name = "tsmi_Tools";
            this.tsmi_Tools.Size = new System.Drawing.Size(59, 21);
            this.tsmi_Tools.Text = "工具(&T)";
            // 
            // tsmi_Calc
            // 
            this.tsmi_Calc.Name = "tsmi_Calc";
            this.tsmi_Calc.Size = new System.Drawing.Size(180, 22);
            this.tsmi_Calc.Text = "计算器(&C)";
            this.tsmi_Calc.Click += new System.EventHandler(this.Tsmi_Calc_Click);
            // 
            // tsmi_Notepad
            // 
            this.tsmi_Notepad.Name = "tsmi_Notepad";
            this.tsmi_Notepad.Size = new System.Drawing.Size(180, 22);
            this.tsmi_Notepad.Text = "记事本(&N)";
            this.tsmi_Notepad.Click += new System.EventHandler(this.Tsmi_Notepad_Click);
            // 
            // tsmi_Help
            // 
            this.tsmi_Help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Helpword,
            this.tsmi_Support,
            this.tsmi_Version});
            this.tsmi_Help.Name = "tsmi_Help";
            this.tsmi_Help.Size = new System.Drawing.Size(61, 21);
            this.tsmi_Help.Text = "帮助(&H)";
            // 
            // tsmi_Helpword
            // 
            this.tsmi_Helpword.Name = "tsmi_Helpword";
            this.tsmi_Helpword.Size = new System.Drawing.Size(180, 22);
            this.tsmi_Helpword.Text = "帮助文档(&H)";
            this.tsmi_Helpword.Click += new System.EventHandler(this.Tsmi_Helpword_Click);
            // 
            // tsmi_Support
            // 
            this.tsmi_Support.Name = "tsmi_Support";
            this.tsmi_Support.Size = new System.Drawing.Size(180, 22);
            this.tsmi_Support.Text = "技术支持(&C)";
            this.tsmi_Support.Click += new System.EventHandler(this.Tsmi_Support_Click);
            // 
            // tsmi_Version
            // 
            this.tsmi_Version.Name = "tsmi_Version";
            this.tsmi_Version.Size = new System.Drawing.Size(180, 22);
            this.tsmi_Version.Text = "版本信息(&V)";
            this.tsmi_Version.Click += new System.EventHandler(this.Tsmi_Version_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // notifyWCS
            // 
            this.notifyWCS.Text = "WCS";
            this.notifyWCS.Visible = true;
            this.notifyWCS.DoubleClick += new System.EventHandler(this.NotifyWCS_DoubleClick);
            // 
            // NicontextMeun
            // 
            this.NicontextMeun.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Show,
            this.tsmi_Hide,
            this.tsmi_Quit});
            this.NicontextMeun.Name = "NicontextMeun";
            this.NicontextMeun.Size = new System.Drawing.Size(101, 70);
            // 
            // tsmi_Show
            // 
            this.tsmi_Show.Name = "tsmi_Show";
            this.tsmi_Show.Size = new System.Drawing.Size(100, 22);
            this.tsmi_Show.Text = "显示";
            this.tsmi_Show.Click += new System.EventHandler(this.Tsmi_Show_Click);
            // 
            // tsmi_Hide
            // 
            this.tsmi_Hide.Name = "tsmi_Hide";
            this.tsmi_Hide.Size = new System.Drawing.Size(100, 22);
            this.tsmi_Hide.Text = "隐藏";
            this.tsmi_Hide.Click += new System.EventHandler(this.Tsmi_Hide_Click);
            // 
            // tsmi_Quit
            // 
            this.tsmi_Quit.Name = "tsmi_Quit";
            this.tsmi_Quit.Size = new System.Drawing.Size(100, 22);
            this.tsmi_Quit.Text = "退出";
            this.tsmi_Quit.Click += new System.EventHandler(this.Tsmi_Quit_Click);
            // 
            // TabControl1
            // 
            this.TabControl1.AutoScroll = true;
            this.TabControl1.BackColor = System.Drawing.SystemColors.Control;
            this.TabControl1.CausesValidation = false;
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.TabControl1.MenuRenderer = toolStripSystemRenderer2;
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.TabControl1.Size = new System.Drawing.Size(791, 305);
            this.TabControl1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.TabControl1.TabBackHighColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.TabControl1.TabBackLowColorDisabled = System.Drawing.SystemColors.GradientInactiveCaption;
            this.TabControl1.TabBorderEnhanced = true;
            this.TabControl1.TabBorderEnhanceWeight = MdiTabControl.TabControl.Weight.Soft;
            this.TabControl1.TabCloseButtonImage = null;
            this.TabControl1.TabCloseButtonImageDisabled = null;
            this.TabControl1.TabCloseButtonImageHot = null;
            this.TabControl1.TabCloseButtonSize = new System.Drawing.Size(14, 14);
            this.TabControl1.TabHeight = 20;
            this.TabControl1.TabIconSize = new System.Drawing.Size(0, 0);
            this.TabControl1.TabIndex = 37;
            this.TabControl1.TabOffset = -8;
            this.TabControl1.TabPadLeft = 20;
            this.TabControl1.TabPadRight = 7;
            this.TabControl1.TabTop = 1;
            this.TabControl1.TopSeparator = false;
            this.TabControl1.GetTabRegion += new MdiTabControl.TabControl.GetTabRegionEventHandler(this.TabControl1_GetTabRegion);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(791, 305);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.TabControl1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMain";
            this.Text = "WCSControl";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.NicontextMeun.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        protected MdiTabControl.TabControl TabControl1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmi_System;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Restart;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Exit;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Tools;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Help;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.NotifyIcon notifyWCS;
        private System.Windows.Forms.ContextMenuStrip NicontextMeun;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Calc;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Notepad;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Helpword;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Support;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Version;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Show;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Hide;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Quit;
    }
}

