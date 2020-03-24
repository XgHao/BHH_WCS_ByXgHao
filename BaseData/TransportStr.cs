using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseData
{
    /// <summary>
    /// 输送机相关实体
    /// </summary>
    public class TransportStr
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string SSJID { get; set; }
        /// <summary>
        /// 对应下位ID
        /// </summary>
        public string BTID { get; set; }
        /// <summary>
        /// 输送机类型
        /// </summary>
        public string DTYPE { get; set; }
        /// <summary>
        /// 任务号
        /// </summary>
        public string ZXRWH { get; set; }
        /// <summary>
        /// 到位信号
        /// </summary>
        public string DWXH { get; set; }
        /// <summary>
        /// 空闲标识
        /// </summary>
        public string KXBZ { get; set; }
        /// <summary>
        /// 托盘条码
        /// </summary>
        public string TRAYCODE { get; set; }
        /// <summary>
        /// 校验码
        /// </summary>
        public string JYM { get; set; }
        /// <summary>
        /// 出库输送机标识 
        /// 1：回流
        /// 2：出库
        /// </summary>
        public string BFLAG { get; set; }
        /// <summary>
        /// 输送机所属巷道
        /// </summary>
        public string ALLEYID { get; set; }
        /// <summary>
        /// 源站台
        /// </summary>
        public string SAllryId { get; set; }
        /// <summary>
        /// 记录输送机对应PLC
        /// </summary>
        public string VAR1 { get; set; }
        /// <summary>
        /// 备注输送机
        /// </summary>
        public string VAR2 { get; set; }
        /// <summary>
        /// DB50起始位置
        /// </summary>
        public string VAR3 { get; set; }
        /// <summary>
        /// DB54起始位置
        /// </summary>
        public string VAR4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VAR5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SSRWLX { get; set; }
        /// <summary>
        /// 保存设备id第一个字符
        /// </summary>
        public string SSJIDhead { get; set; }
        /// <summary>
        /// 保存调度任务id
        /// </summary>
        public string Taskid { get; set; }
        /// <summary>
        /// 保存12个字节的位
        /// </summary>
        public BitArray Arr { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public string Isfinish { get; set; }
        /// <summary>
        /// 强制完成
        /// </summary>
        public string Focusfinish { get; set; }
        /// <summary>
        /// 尾箱信号
        /// </summary>
        public string Islast { get; set; }
        /// <summary>
        /// 箱数
        /// </summary>
        public double Boxnum { get; set; }
        /// <summary>
        /// 拆数量
        /// </summary>
        public int Picknum { get; set; }
        /// <summary>
        /// 分配数量【出库数量】
        /// </summary>
        public int Assignnum { get; set; }
        /// <summary>
        /// 道口号
        /// </summary>
        public int Shelvway { get; set; }
        /// <summary>
        /// 是否报警
        /// </summary>
        public bool Isalarm { get; set; }
        /// <summary>
        /// 报警对象
        /// </summary>
        public SSJAlarm SSJAlarm { get; set; }
        /// <summary>
        /// 输送机上货物重量
        /// </summary>
        public double Weight { get; set; }
    }
}
