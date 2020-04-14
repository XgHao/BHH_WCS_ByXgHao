using BaseData;
using COMRW;
using DataOperate;
using LedDisplay;
using LOG;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeSpanCount;

namespace ComResolution
{
    public class CRLTRANSControl : CRLBase
    {
        /// <summary>
        /// 一楼输送机字节
        /// </summary>
        private byte[] firstbuffer;
        /// <summary>
        /// 一楼电机字节
        /// </summary>
        private byte[] firstdjbuffer;
        /// <summary>
        /// 二楼输送机字节
        /// </summary>
        private byte[] secondbuffer;
        /// <summary>
        /// 二楼电机字节
        /// </summary>
        private byte[] seconddjbuffer;

        private PerformanceTimer pft = new PerformanceTimer();
        public static Log logWrite = new Log("系统日志", @".\输送机日志\");

        //将消息发送给界面
        public delegate void NotifyDg(string msg);
        public event NotifyDg NotifyEvent;

        public delegate void DescTransportInfoDG(TransportStr si);
        public event DescTransportInfoDG DescTransportInfoEvent;

        public delegate void GetEquipMentInfoDG(List<TransportStr> transportStrs);
        public event GetEquipMentInfoDG GetEquipMentInfoEvent;

        //对输送机下达任务
        public delegate void IssuedDG(byte[] firstbuffer, byte[] firstdjbuffer, byte[] secondbuffer, byte[] seconddjbuffer);
        public event IssuedDG IssuedEvent;

        //输送机已下发任务号
        Dictionary<string, string> HasIssued = new Dictionary<string, string>();

        /// <summary>
        /// 写db块
        /// </summary>
        private int dbnumber = 55;
        /// <summary>
        /// 开始字节
        /// </summary>
        private int value = 0;
        /// <summary>
        /// 电机db块
        /// </summary>
        private int djdbnumber = 54;

        LedServer led = new LedServer();
        /// <summary>
        /// 用于记录给LED发送过的信息
        /// </summary>
        Dictionary<string, string> dcsaveledstr = new Dictionary<string, string>();
        /// <summary>
        /// 获取到的消息
        /// </summary>
        string rcgmsg;

        /// <summary>
        /// 初始化输送机对应参数
        /// </summary>
        /// <returns></returns>
        public bool TransDeviceInit()
        {
            DataSet ds = DataTrans.D_GetDeviceSSJ();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //获取设备编号第一个字母，用于与绑定IP地址
                    _SSJDhead = ds.Tables[0].Rows[i]["VAR1"].ToString().Substring(0, 1);
                    string _ip = ConfigurationManager.AppSettings[_SSJDhead].ToString();

                    //需要改进，将端口，DB块，端口等也进行构造函数初始化l
                    if (!PLCList.ContainsKey(_SSJDhead))
                    {
                        PLCList.Add(_SSJDhead, new RWLOPCServerl(_ip));
                        PLCFlag.Add(_SSJDhead, false);
                        PLCList[_SSJDhead].Connect();
                    }
                    if (!HsPLCList.ContainsKey(_SSJDhead))
                    {
                        HsPLCList.Add(_SSJDhead, new HsControlServer(_ip));
                        bool flag = HsPLCList[_SSJDhead].HsServerConnect();
                        string res = flag ? "成功" : "失败";
                        NotifyEvent?.Invoke($"PLC{_ip}连接{res}");
                    }

                    TransportStr tp = new TransportStr();
                    tp.SSJID = ds.Tables[0].Rows[i]["SSJID"].ToString();
                    tp.BTID = ds.Tables[0].Rows[i]["BTID"].ToString();
                    tp.DTYPE = ds.Tables[0].Rows[i]["DTYPE"].ToString();
                    tp.ZXRWH = ds.Tables[0].Rows[i]["ZXRWH"].ToString();
                    tp.DWXH = ds.Tables[0].Rows[i]["DWXH"].ToString();
                    tp.KXBZ = ds.Tables[0].Rows[i]["KXBZ"].ToString();
                    tp.TRAYCODE = ds.Tables[0].Rows[i]["TRAYCODE"].ToString();
                    tp.JYM = ds.Tables[0].Rows[i]["JYM"].ToString();
                    tp.BFLAG = ds.Tables[0].Rows[i]["BFLAG"].ToString();
                    tp.ALLEYID = ds.Tables[0].Rows[i]["ALLEYID"].ToString();
                    //tp.ALLEYID = "";
                    tp.VAR1 = ds.Tables[0].Rows[i]["VAR1"].ToString();
                    tp.VAR2 = ds.Tables[0].Rows[i]["VAR2"].ToString();
                    tp.VAR3 = ds.Tables[0].Rows[i]["VAR3"].ToString();
                    tp.VAR4 = ds.Tables[0].Rows[i]["VAR4"].ToString();
                    tp.VAR5 = ds.Tables[0].Rows[i]["VAR5"].ToString();
                    tp.SSRWLX = ds.Tables[0].Rows[i]["SSRWLX"].ToString();
                    tp.SSJIDhead = _SSJDhead;
                    lsTransport.Add(tp);
                }
                return true;
            }
            return false;
        }

        public void Run()
        {
            ComTask = Task.Run(() =>
            {
                while (true)
                {
                    //下发任务
                    IssusedTask();
                    Thread.Sleep(500);
                }
            });
        }

        /// <summary>
        /// 下发任务
        /// </summary>
        private void IssusedTask()
        {
        }
    }
}
