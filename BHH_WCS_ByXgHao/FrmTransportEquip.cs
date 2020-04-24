using BLL;
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
    public partial class FrmTransportEquip : Form
    {
        readonly BLLTransport bt = new BLLTransport();
        public FrmTransportEquip()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmTransportEquip_Load(object sender, EventArgs e)
        {
            bt.NotifyEvent += msg => ShowText(rbinfo, msg);
            bt.run();
        }


        private void ShowText(RichTextBox rtb,string msg)
        {
            if (rtb.InvokeRequired)
            {
                rtb.Invoke(new Action(() => ShowText(rtb, msg)));
                return;
            }

            if (!string.IsNullOrEmpty(msg))
            {
                if (msg.Contains("失败") || msg.Contains("故障") || msg.Contains("任务号:0")) 
                {
                    rtb.SelectionColor = Color.Red;
                }
                else
                {
                    rtb.SelectionColor = Color.Black;
                }
                if (rtb.TextLength > 5000)
                {
                    rtb.Clear();
                }
                if (!string.IsNullOrEmpty(msg))
                {
                    rtb.AppendText($"{DateTime.Now} {msg}{Environment.NewLine}");
                }
            }
        }
    }
}
