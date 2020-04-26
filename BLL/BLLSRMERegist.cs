using BaseData;
using ComResolution;
using LOG;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BLL
{
    public class BLLSRMERegist : BLLBase
    {
        CRLSRMControl crs = new CRLSRMControl();
        SRMStr srs;
        private Log log;

        private string m_MessageRecv;


        public delegate void NotifyShow(string type, string msg);
        public event NotifyShow NotifyShowEvent;

        public Timer CheckConnectTimer { get; } = new Timer();
        public Timer GetSCStatusTimer { get; } = new Timer();


        public BLLSRMERegist(SRMStr sr)
        {
            srs = new SRMStr
            {
                Name = sr.Name,
                Port = sr.Port,
                Ip = sr.Ip
            };

            log = new Log($"业务逻辑{sr.Name}", @".\堆垛机日志\");
            crs.NotifyEvent += NotifyCommand;

            CheckConnectTimer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["CheckConnectTimer"]);
            //若连接失败则检测网络直到连接上了
            CheckConnectTimer.Elapsed += CheckConnectTimer_Elapsed;

            GetSCStatusTimer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["GetStatusTime"]);
            //获取堆垛机信息Timer事件
            GetSCStatusTimer.Elapsed += GetSCStatusTimer_Elapsed;
        }


        public void Run()
        {
            //判断连接
            if (crs.Connect(srs.Ip, srs.Port))
            {
                GetSCStatusTimer.Start();
                NotifyShowEvent?.Invoke("C", $"连接成功，IP地址{srs.Ip},堆垛机名称{srs.Name},端口{srs.Port}");
            }
            else
            {
                CheckConnectTimer.Start();
                NotifyShowEvent?.Invoke("C", $"连接失败，IP地址{srs.Ip},堆垛机名称{srs.Name},端口{srs.Port}");
            }
        }


        /// <summary>
        /// 连接成功，读取堆垛机信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetSCStatusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (srs.Port == "4000")
            {
                if (!crs.Send(MessageHeader_4000))
                {
                    CheckConnectTimer.Start();
                    GetSCStatusTimer.Stop();
                }
            }
            else if (srs.Port == "6000") 
            {
                if (srs.Name == "HC001") 
                {
                    if (!crs.Send(MessageHeaderHC_6000)) 
                    {
                        CheckConnectTimer.Start();
                        GetSCStatusTimer.Stop();
                    }
                }
                else
                {
                    if (!crs.Send(MessageHeader_6000)) 
                    {
                        CheckConnectTimer.Start();
                        GetSCStatusTimer.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// 如果连接失败，定时检测网络，网络正常则自动连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckConnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<CRCObject> crclist = new List<CRCObject>();
            CRCObject findCob = crclist.Find(info => info.ScNo == srs.Name && info.Port == int.Parse(srs.Port));

            if (crs.Connect(srs.Ip, srs.Port))
            {
                GetSCStatusTimer.Start();
                CheckConnectTimer.Stop();
                if (findCob != null) 
                {
                    findCob.connectstatus = 1;
                }
                NotifyShowEvent?.Invoke("C", $"检测连接成功，IP地址{srs.Ip},堆垛机编号{srs.Name},端口{srs.Port}");
            }
            else
            {
                NotifyShowEvent?.Invoke("C", $"检测连接失败，IP地址{srs.Ip},堆垛机编号{srs.Name},端口{srs.Port}");
                if (findCob != null) 
                {
                    findCob.connectstatus = 0;
                }
            }
        }



        /// <summary>
        /// 方法实现将连接状况写入日志
        /// </summary>
        /// <param name="command"></param>
        /// <param name="state"></param>
        public void NotifyCommand(string command, object state)
        {
            string strMsg = string.Empty;
            if (command == "RecvData") 
            {
                byte[] bty = state as byte[];
                for (int i = 0; i < 256; i++)
                {
                    strMsg += $"{bty[i]};";
                }
                #region 对应byte待修改2017/11/13
                if (m_MessageRecv != $"任务号:{bty[20]},{bty[21]}；指令代码:{bty[22]}；函数模式:{bty[23]}；堆垛机模式:{bty[38]}；动作点:{bty[26]}；巷道:{bty[27]}；行:{bty[28]}；列:{bty[29]}；列前后:{bty[30]}；高度:{bty[31]}；深度:{bty[32]}") ;
                {
                    m_MessageRecv = $"任务号:{bty[20]},{bty[21]}；指令代码:{bty[22]}；函数模式:{bty[23]}；堆垛机模式:{bty[38]}；动作点:{bty[26]}；巷道:{bty[27]}；行:{bty[28]}；列:{bty[29]}；列前后:{bty[30]}；高度:{bty[31]}；深度:{bty[32]}";
                    log.WriteLog(m_MessageRecv);
                }
                #endregion
            }
            else if (command == "Break" || command == "Connect")
            {
                log.WriteLog($"{command}:{state}");
            }
        }
    }
}
