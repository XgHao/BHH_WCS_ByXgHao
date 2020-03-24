using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseData
{
    /// <summary>
    /// 输送机报警
    /// </summary>
    public class SSJAlarm
    {
        /// <summary>
        /// 左超报警
        /// </summary>
        public bool LeftAlarm { get; set; }
        /// <summary>
        /// 右超报警
        /// </summary>
        public bool RightAlarm { get; set; }
        /// <summary>
        /// 超高报警
        /// </summary>
        public bool HighAlarm { get; set; }
        /// <summary>
        /// 前超报警
        /// </summary>
        public bool FrontAlarm { get; set; }
        /// <summary>
        /// 后超报警
        /// </summary>
        public bool BackAlarm { get; set; }
        /// <summary>
        /// 条码错误报警
        /// </summary>
        public bool CodeAlarm { get; set; }
        /// <summary>
        /// 间隙报警
        /// </summary>
        public bool SpaceAlarm { get; set; }
    }
}
