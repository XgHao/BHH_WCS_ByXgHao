using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace BaseData
{
    /// <summary>
    /// 收获详细记录表
    /// </summary>
    public class RECDETAIL
    {
        /// <summary>
        /// 药监码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 产品名
        /// </summary>
        public string ProcudtName { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string EUnit { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string LotInfo { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public string Num { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public string ProductDate { get; set; }

        /// <summary>
        /// 通知单头ID
        /// </summary>
        public string PoID { get; set; }

        /// <summary>
        /// 通知单明细ID
        /// </summary>
        public string PoDetailID { get; set; }

        /// <summary>
        /// 接收单ID
        /// </summary>
        public string ReceID { get; set; }

        /// <summary>
        /// 产品状态
        /// 【1=待检】【2=合格】【不合格】
        /// </summary>
        public string ProductStatus { get; set; }

        /// <summary>
        /// 【1=合格】【2=不合格】
        /// </summary>
        public string YPStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CreateManID { get; set; }

        /// <summary>
        /// 数量（一箱里放的数量）
        /// </summary>
        public string SKUNum { get; set; }

        /// <summary>
        /// 上架道口(01-99)
        /// </summary>
        public string WAYID { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 是否计数
        /// </summary>
        public string IsCount { get; set; }

        /// <summary>
        /// 是否有尾箱标识
        /// </summary>
        public string IsLast { get; set; }

        /// <summary>
        /// 已扫描数量
        /// </summary>
        public string ScanNum { get; set; }
    }
}
