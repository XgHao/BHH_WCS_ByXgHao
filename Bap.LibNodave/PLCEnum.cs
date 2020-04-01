using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bap.LibNodave
{
    public static class PLCEnum{
        
        /// <summary>
        /// 定义的PLC的内存区域，对于simens，是这样定义的
        /// I:离散输入和映像寄存器
        /// Q:离散输出和映像寄存器
        /// M:内部内存位
        /// SM:特殊内存位（SM0 - SM29为只读内存区）
        /// V:变量内存
        /// T:定时器当前值和定时器位
        /// C:计数器当前值和计数器位
        /// HC:高速计数器当前位
        /// AI:模拟输入
        /// AQ:模拟输出
        /// L:局部变量内存
        /// </summary>
        public enum PlcMemoryArea
        {
            /// <summary>
            /// PLC中的系统信息[only s7-200 family]
            /// </summary>
            SysInfo = 0x3,

            /// <summary>
            /// 系统的标志位信息[only s7-200 family]
            /// </summary>
            SysFlags = 0x5,

            /// <summary>
            /// 模拟输入[analog inputs:only s7-200 family]
            /// </summary>
            AnaIn = 0x6,

            /// <summary>
            /// 模拟输出[analog outputs:only s7-200 family]
            /// </summary>
            AnaOut = 0x7,

            /// <summary>
            /// 直接外围访问，direct peripheral access
            /// </summary>
            P = 0x80,

            /// <summary>
            /// 输入
            /// </summary>
            Inputs = 0x81,

            /// <summary>
            /// 输出
            /// </summary>
            Outputs = 0x82,

            /// <summary>
            /// 标志
            /// </summary>
            Flags = 0x83,

            /// <summary>
            /// PLC的data blocks，地球人都知道主要玩这个
            /// </summary>
            DB = 0x84,

            /// <summary>
            /// 实例数据块，instance data blocks
            /// </summary>
            DI = 0x85,

            /// <summary>
            /// 局部变量内存
            /// </summary>
            Local = 0x86,

            /// <summary>
            /// 变量内存
            /// </summary>
            V = 0x87,

            /// <summary>
            /// 计数器当前值和计数器位
            /// </summary>
            Counter = 28,

            /// <summary>
            /// 定时器当前值和定时器位
            /// </summary>
            Timer = 29,

            /// <summary>
            /// IEC计数器当前值和计数器位[only s7-200 family]
            /// </summary>
            Counter200 = 30,

            /// <summary>
            /// 定时器当前值和定时器位[only s7-200 family]
            /// </summary>
            Timer200 = 31
        }


        public enum S7BlockType
        {
            OB = '8',
            DB = 'A',
            SDB = 'B',
            FC = 'C',
            SFC = 'D',
            FB = 'E',
            SFB = 'F'
        }
    }
}
