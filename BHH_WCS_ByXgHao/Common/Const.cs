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
        public const int WM_SYSCOMMAND  = 0X112;

        public const int SC_MINIMIZE = 0XF020;

        public const int SC_CLOSE = 0XF060;

        public const int SC_RESTORE  = 0XF120;

        /// <summary>
        /// 记事本需要的常量
        /// </summary>
        public const uint WM_SETTEXT  = 0X000C;

        /// <summary>
        /// 自动关闭弹出框需要的常量
        /// </summary>
        public const int WM_CLOSE = 0x10;

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
                    return $"Can't find [{nameof(MainName)}]";
                    
                }
            }
        }

        public static string FormStatus
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["FormStatus"];
                }
                catch (ConfigurationErrorsException)
                {
                    return $"Can't find [{nameof(FormStatus)}]";
                }
            }
        }
    }
}
