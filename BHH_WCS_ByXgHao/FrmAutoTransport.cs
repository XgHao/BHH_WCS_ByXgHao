using ComResolution;
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
    public partial class FrmAutoTransport : Form
    {
        readonly CRLSocket bs = new CRLSocket();
        public FrmAutoTransport()
        {
            InitializeComponent();
        }

        private void FrmAutoTransport_Load(object sender, EventArgs e)
        {
            bs.NotifyShowEvent += (type, msg) =>
            {
                if (type == "C") 
                {
                    ShowText(rtb_Con, msg);
                }
                else
                {
                    ShowText(rtb_Task, msg);
                }
            };
        }


        private void ShowText(RichTextBox rb,string msg)
        {
            if (rb.InvokeRequired)
            {
                rb.Invoke(new Action(() => ShowText(rb, msg)));
                return;
            }

            if (msg.Contains("失败") || msg.Contains("故障"))
            {
                rb.SelectionColor = Color.Red;
            }
            else
            {
                rb.SelectionColor = Color.Black;
            }
            if (rb.Text.Length > 5000)
            {
                rb.Clear();
            }
            rb.AppendText($"{DateTime.Now}:{msg}{Environment.NewLine}");
        }
    }
}
