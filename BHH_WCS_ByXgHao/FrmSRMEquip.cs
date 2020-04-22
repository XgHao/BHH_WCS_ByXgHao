using BaseData;
using BHH_WCS_ByXgHao.Common;
using BLL;
using ComResolution;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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

        private delegate void PictureCallBack(List<CRCObject> lscro, List<CraneStr> lscrs);

        private delegate void RichBoxCallBack(RichTextBox rb, string msg);

        private BLLCrane bcr = new BLLCrane();

        private void FrmSRMEquip_Load(object sender, EventArgs e)
        {
            bcr.NotifyEvent += bsb_EDNotify;
        }

        private void bsb_EDNotify(string type, string msg)
        {
            if (type == "C") 
            {
                ShowText(rtb_conn, msg);
            }
            else
            {
                ShowText(rtb_msg, msg);
            }
        }

        private void ShowText(RichTextBox rb, string msg)
        {
            if (rb.InvokeRequired)
            {
                //包一层
                Invoke(new Action(() =>
                {
                    ShowText(rb, msg);
                }));
                return;
            }

            //具体操作
            if (msg.Contains("失败") || msg.Contains("故障")) 
            {
                rb.SelectionColor = Color.Red;
            }
            else
            {
                rb.SelectionColor = Color.Black;
            }
            if (rb.TextLength > 5000) 
            {
                rb.Clear();
            }
            rb.AppendText($"{DateTime.Now}:{msg}{Environment.NewLine}");
        }
    }
}
