using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHH_WCS_ByXgHao.Common
{
    /// <summary>
    /// 系统应用
    /// </summary>
    public static class SystemApplication
    {
        /// <summary>
        /// 打开系统应用
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="useShellExecute"></param>
        /// <param name="redirectStandardInput"></param>
        /// <param name="redirectStandardOutput"></param>
        /// <returns>返回程序对象，失败则返回NULL</returns>
        public static Process Open(string filename, bool useShellExecute = false, bool redirectStandardInput = true, bool redirectStandardOutput = true)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = filename,
                UseShellExecute = useShellExecute,
                RedirectStandardInput = redirectStandardInput,
                RedirectStandardOutput = redirectStandardOutput
            };
            Process process = new Process
            {
                StartInfo = processStartInfo
            };
            try
            {
                process.Start();
                return process;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
