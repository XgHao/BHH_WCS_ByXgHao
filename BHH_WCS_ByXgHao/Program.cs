using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BHH_WCS_ByXgHao
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }

        /// <summary>
        /// 重启WCS程序
        /// </summary>
        public static void ProRestart()
        {
            System.Diagnostics.ProcessStartInfo cp = new System.Diagnostics.ProcessStartInfo();
            cp.FileName = Application.ExecutablePath;
            cp.Arguments = "GYCWS.exe";
            cp.UseShellExecute = true;
            System.Diagnostics.Process.Start(cp);
        }
    }
}
