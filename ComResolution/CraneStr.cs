using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComResolution
{
    /// <summary>
    /// 堆垛机类
    /// </summary>
    public class CraneStr
    {
        /// <summary>
        /// 堆垛机id
        /// </summary>
        public string Btid { get; set; }
        /// <summary>
        /// 操作方式
        /// </summary>
        public string Czfs { get; set; }
        /// <summary>
        /// 阶段标识
        /// </summary>
        public string Jdbz { get; set; }
        /// <summary>
        /// 作业方式
        /// </summary>
        public string Zyfs { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public string Dqzt { get; set; }
        /// <summary>
        /// 任务号
        /// </summary>
        public int Zxrwh { get; set; }
        /// <summary>
        /// 入库目标有货
        /// </summary>
        public string Rkyh { get; set; }
        /// <summary>
        /// 出库目标有货
        /// </summary>
        public string Ckwh { get; set; }
        /// <summary>
        /// 有货待机
        /// </summary>
        public string Yhdj { get; set; }
        /// <summary>
        /// 无货待机
        /// </summary>
        public string Whdj { get; set; }
        /// <summary>
        /// 当前排号
        /// </summary>
        public string Dqph { get; set; }
        /// <summary>
        /// 当前列号
        /// </summary>
        public string Dqlh { get; set; }
        /// <summary>
        /// 当前层号
        /// </summary>
        public string Dqch { get; set; }
        /// <summary>
        /// 目标排号
        /// </summary>
        public string Mbph { get; set; }
        /// <summary>
        /// 目标列号
        /// </summary>
        public string Mblh { get; set; }
        /// <summary>
        /// 目标层号
        /// </summary>
        public string Mbch { get; set; }
        /// <summary>
        /// 当前巷道
        /// </summary>
        public string Dqxdh { get; set; }
        /// <summary>
        /// 故障报警
        /// </summary>
        public string Alarm { get; set; }
        /// <summary>
        /// 预留字段1
        /// </summary>
        public string Var1 { get; set; }
        /// <summary>
        /// 预留字段2
        /// </summary>
        public string Var2 { get; set; }
        /// <summary>
        /// 预留字段3
        /// </summary>
        public string Var3 { get; set; }
        /// <summary>
        /// 预留字段4
        /// </summary>
        public string Var4 { get; set; }
        /// <summary>
        /// 预留字段5
        /// </summary>
        public string Var5 { get; set; }
    }
}
