using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BHH_WCS_ByXgHao.Common
{
    internal static class API
    {
        #region 记事本
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, string lParam);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        #endregion

        #region 自动关闭弹出框
        [DllImport("User32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
        internal extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal extern static int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        #endregion
    }
}
