using BaseData;
using COMRW;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComResolution
{
    public class CRLBase
    {
        public static List<TransportStr> lsTransport = new List<TransportStr>();
        public static List<TransportStr> lsAotuTransport = new List<TransportStr>();
        /// <summary>
        /// PLC列表(输送机)
        /// </summary>
        public static Dictionary<string, RWLOPCServerl> PLCList = new Dictionary<string, RWLOPCServerl>();
        /// <summary>
        /// PLC列表(输送机)
        /// </summary>
        public static Dictionary<string, HsControlServer> HsPLCList = new Dictionary<string, HsControlServer>();
        /// <summary>
        /// 堆垛机PLC列表
        /// </summary>
        public static Dictionary<string, HsControlServer> cPLCList = new Dictionary<string, HsControlServer>();
        /// <summary>
        /// PLC状态
        /// </summary>
        public Dictionary<string, bool> PLCFlag = new Dictionary<string, bool>();

        public List<string> PLCReLink = new List<string>();

        public string _SSJDhead;

        /// <summary>
        /// 通讯线程
        /// </summary>
        public Thread ComThread;

        /// <summary>
        /// 堆垛机实体类List
        /// </summary>
        public static List<CRCObject> CrcList = new List<CRCObject>();
        /// <summary>
        /// 堆垛机状态实体类List
        /// </summary>
        public static List<CraneStr> CraneStrList = new List<CraneStr>();
        /// <summary>
        /// Socket传输实体类List
        /// </summary>
        public static List<CRCSCKObject> CrcSckList = new List<CRCSCKObject>();
        /// <summary>
        /// 用于记录箱式线批次到达数量
        /// </summary>
        public static List<ProductCountInfo> ProcInfoList = new List<ProductCountInfo>();
        /// <summary>
        /// 用于存储分拣产品信息
        /// </summary>
        public static Hashtable HashPickInfo = new Hashtable();
        /// <summary>
        /// 小车回调
        /// </summary>
        public static Dictionary<string, string> DicAgvCallBack = new Dictionary<string, string>();
    }
}
