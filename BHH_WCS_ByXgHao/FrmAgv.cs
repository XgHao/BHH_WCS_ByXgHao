﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;

namespace BHH_WCS_ByXgHao
{
    public partial class FrmAgv : Form
    {
        public FrmAgv()
        {
            InitializeComponent();
        }

        private void FrmAgv_Load(object sender, EventArgs e)
        {
            BLLAgvBase agvBase = new BLLAgvBase();
            BLLAgvBase.bll.NotifyEvent += (type, msg) =>
            {
                if (type == "R") 
                {
                    ShowText(rb_RcvAgvMsg, msg);
                }
                else
                {
                    ShowText(rb_SendAgvMsg, msg);
                }
            };
            BLLAgvBase.bll.Run();
        }

        private void ShowText(RichTextBox rb, string msg)
        {
            if (rb.InvokeRequired)
            {
                rb.Invoke(new Action(() =>
                {
                    ShowText(rb, msg);
                }));
                return;
            }

            if (string.IsNullOrEmpty(msg)) 
            {
                return;
            }
            if (msg.Contains("失败") || msg.Contains("故障") || msg.Contains("异常")) 
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
