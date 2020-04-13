using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComResolution
{
    public class ProductCountInfo
    {
        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 批号
        /// </summary>
        public string LotInfo { get; set; }

        /// <summary>
        /// 已到数量
        /// </summary>
        public int GetNum { get; set; }
    }
}
