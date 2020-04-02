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

        /// <summary>
        /// PLC通讯协议
        /// </summary>
        public enum Protocols
        {
            /// <summary>
            /// MPI for S7 300/400 
            /// </summary>
            MPI = 0,

            /// <summary>
            /// MPI for S7 300/400【安德鲁电缆】
            /// </summary>
            MPI2 = 1,

            /// <summary>
            /// MPI for S7 300/400 Step 7 Version experimental
            /// </summary>
            MPI3 = 2,

            /// <summary>
            /// MPI for S7 300/400 "Andrew's version" with STX 
            /// </summary>
            MPI4 = 3,

            /// <summary>
            /// PPI for S7 200 
            /// </summary>
            PPI = 10,

            /// <summary>
            /// S5 via programming interface
            /// </summary>
            AS511 = 20,

            /// <summary>
            ///  use s7onlinx.dll for transport
            /// </summary>
            S7OnLine = 50,

            /// <summary>
            /// ISO over TCP
            /// </summary>
            ISOTCP = 122,

            /// <summary>
            /// 使用CP243的工业以太网
            /// </summary>
            ISOTCP243 = 123,

            /// <summary>
            /// MPI with IBH NetLink MPI to ethernet gateway 
            /// </summary>
            MPI_IBH = 223,

            /// <summary>
            /// PPI with IBH NetLink PPI to ethernet gateway
            /// </summary>
            PPI_IBH = 224,

            /// <summary>
            /// 使用Libnodave定义的协议数据单元PDU（Protocol Data Unit）
            /// </summary>
            UserTransport = 255
        }

        /// <summary>
        /// ProfiBus speed定义。MPI串口通信的波特率，并非PLC和PC的通信速度
        /// </summary>
        public enum ProfiBusSpeed
        {
            Speed9k = 0,
            Speed19k = 1,
            Speed187k = 2,
            Speed500k = 3,
            Speed1500k = 4,
            Speed45k = 5,
            Speed93k = 6
        }
    }
}
