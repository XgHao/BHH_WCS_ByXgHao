using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHH_WCS_ByXgHao.Common
{
    /// <summary>
    /// 常量
    /// </summary>
    public static class Const
    {
        public static int WM_SYSCOMMAND { get; } = 0X112;

        public static int SC_MINIMIZE { get; } = 0XF020;

        public static int SC_CLOSE { get; } = 0XF060;

        public static int SC_RESTORE { get; } = 0XF120;

        /// <summary>
        /// 记事本需要的常量
        /// </summary>
        public static uint WM_SETTEXT { get; } = 0X000C;

        public static string MainName
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["MainName"];
                }
                catch (ConfigurationErrorsException)
                {
                    return "MainName";
                }
            }
        }

        public static string FormStatus
        {
            get
            {
                return ConfigurationManager.AppSettings["FormStatus"];
            }
        }
    }
}
