using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using BHH_WCS_ByXgHao.Common;
using System.Runtime.InteropServices;

namespace BHH_WCS_ByXgHao
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
        }

        /// <summary>
        /// 重写关闭窗体事件
        /// 禁止关闭
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            ShowInTaskbar = false;
            ShowIcon = false;
            WindowState = FormWindowState.Minimized;
            e.Cancel = true;
        }

        /// <summary>
        /// 处理windows消息
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Const.WM_SYSCOMMAND) 
            {
                int wParam = (int)m.WParam;
                //拦截 除最小化、恢复、关闭以外的消息
                if (wParam != Const.SC_MINIMIZE && wParam != Const.SC_CLOSE && wParam != Const.SC_RESTORE) 
                {
                    return;
                }
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// 系统--重启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsmi_Restart_Click(object sender, EventArgs e)
        {
            Program.ProRestart();
        }

        /// <summary>
        /// 系统--退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsmi_Exit_Click(object sender, EventArgs e)
        {
            CloseCtiServer();
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        private void CloseCtiServer()
        {
            Application.ExitThread();
            Close();
            Environment.Exit(0);
        }

        /// <summary>
        /// 工具--计算器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsmi_Calc_Click(object sender, EventArgs e)
        {
            //启动计算器
            if (SystemApplication.Open("calc.exe") == null) 
            {
                MessageBox.Show("启动失败");
            }
        }

        /// <summary>
        /// 工具--记事本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsmi_Notepad_Click(object sender, EventArgs e)
        {
            //启动记事本
            var proNotepad = SystemApplication.Open("notepad.exe");
            if (proNotepad == null)
            {
                MessageBox.Show("启动失败");
                return;
            }

            //调用API，传递数据
            while (proNotepad.MainWindowHandle == IntPtr.Zero) 
            {
                proNotepad.Refresh();
            }
            IntPtr vHandle = API.FindWindowEx(proNotepad.MainWindowHandle, IntPtr.Zero, "Edit", null);

            //传递数据给记事本
            API.SendMessage(vHandle, Const.WM_SETTEXT, 0, "");
        }

        /// <summary>
        /// 帮助--帮助文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsmi_Helpword_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 帮助--技术支持
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsmi_Support_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 帮助--版本信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsmi_Version_Click(object sender, EventArgs e)
        {
            new FrmAbout().Show();
        }

        /// <summary>
        /// 双击通知栏显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyWCS_DoubleClick(object sender, EventArgs e)
        {
            ShowCtiServer();
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        private void ShowCtiServer()
        {
            Show();
            WindowState = FormWindowState.Maximized;
            Activate();
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsmi_Show_Click(object sender, EventArgs e)
        {
            ShowCtiServer();
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsmi_Hide_Click(object sender, EventArgs e)
        {
            HideCtiServer();
        }

        /// <summary>
        /// 隐藏客户端服务
        /// </summary>
        private void HideCtiServer()
        {
            Hide();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tsmi_Quit_Click(object sender, EventArgs e)
        {
            CloseCtiServer();
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// 当标签页请求标签区域重现？
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl1_GetTabRegion(object sender, MdiTabControl.TabControl.GetTabRegionEventArgs e)
        {
            e.Points[1] = new Point(e.TabHeight - 2, 2);
            e.Points[2] = new Point(e.TabHeight + 2, 0);
        }

        /// <summary>
        /// 加载时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                string[] l_form = Const.FormStatus.Split(new char[] { ',' });

                int index = -1;
                for (int i = 0; i < l_form.Length; i++)
                {
                    if (l_form[i] == "1") 
                    {
                        switch (i)
                        {
                            case 0:
                                FrmTransportEquip frmTse = new FrmTransportEquip();
                                TabControl1.TabPages.Add(frmTse);
                                TabControl1.TabPages[frmTse].CloseButtonVisible = false;
                                TabControl1.TabPages.set_IndexOf(TabControl1.TabPages[frmTse], ++index);
                                break;
                            case 1:
                                break;
                            case 2:

                                break;
                            default:
                                break;
                        }
                    }
                }
                if (index > -1) 
                {
                    TabControl1.TabPages[0].Select();
                }
                Text = Const.MainName;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
