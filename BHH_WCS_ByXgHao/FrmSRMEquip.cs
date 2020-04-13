using BHH_WCS_ByXgHao.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BHH_WCS_ByXgHao
{
    public partial class FrmSRMEquip : Form
    {
        public FrmSRMEquip()
        {
            InitializeComponent();
        }

        #region 自动关闭弹出框

        private void StartKiller()
        {
            Timer timer = new Timer { Interval = 500 };
            timer.Tick += (sender, e) =>
            {
                KillMessageBox();
                //停止Timer
                (sender as Timer)?.Stop();
            };
        }

        private void KillMessageBox()
        {
            //根据MessageBox的标题，找到MessageBox的视窗
            IntPtr ptr = API.FindWindow(null, " ");
            if (ptr != IntPtr.Zero) 
            {
                //找到则关闭MessageBox视窗
                API.PostMessage(ptr, Const.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }
        #endregion

    }
}
