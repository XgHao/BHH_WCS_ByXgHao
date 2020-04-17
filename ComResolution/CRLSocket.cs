using LOG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Configuration;
using COMRW;

namespace ComResolution
{
    public class CRLSocket : CRLBase
    {
        //将异常内容反馈给窗体
        public delegate void NotifyShow(string type, string msg);
        public event NotifyShow NotifyShowEvent;

        //获取接收到的字节事件
        public delegate void RcvStrDG(object state);
        public event RcvStrDG RcvStrEvent;

        private readonly CRCSCKObject crsk;
        private readonly CRLSocketConnect crConnect = new CRLSocketConnect();
        private string SCName;
        private string IP;
        private int Port;
        private int connectStatus;
        Log logWrite = new Log("系统日志", @".\Socket日志\");

        private Timer checkTime = null;

        int taskid = 0;
        const int autoSdbnumber = 112;
        const int autoRdbnumber = 113;
        const int dbNumber = 50;
        const int dbLastgo = 27;

        /// <summary>
        /// 用于存储道口产品信息
        /// </summary>
        Hashtable hasallotInfo = new Hashtable();

        /// <summary>
        /// 用于储存正在作业的产品
        /// </summary>
        Hashtable jobProductInfo = new Hashtable();

        public Timer CheckTime
        {
            get
            {
                if (checkTime == null) 
                {
                    checkTime = new Timer();
                }
                return checkTime;
            }
        }


        public void TransDeviceInit()
        {
            string _ip = ConfigurationManager.AppSettings["B"].ToString();
            if (!HsPLCList.ContainsKey("B")) 
            {
                HsPLCList.Add("B", new HsControlServer(_ip));
                PLCFlag.Add("B", false);
                HsPLCList["B"].HsServerConnect();
            }
        }

        public void Run()
        {
            crConnect.NotifyCommandEvent += new CRLSocketConnect.NotifyCommandDG(SocketNotify);
        }

        /// <summary>
        /// Socket获取报文
        /// </summary>
        /// <param name="command"></param>
        /// <param name="state"></param>
        private void SocketNotify(string command, object state)
        {
            if (command.Equals("RecvData"))
            {
                //获取PLC发送过来的信息
                RcvStrEvent?.Invoke(state);

                //获取子报文头部，根据头部判断有多少个子报文任务
                byte[] buffer = new byte[290];
                buffer = state as byte[];
                string getByte = BitConverter.ToString(buffer, 0).Replace("-", string.Empty).ToUpper();

                logWrite.WriteLog($"接受报文{getByte}");

                //内部最多五个子报文
                for (int i = 0; i < 5; i++)
                {
                    byte[] tmpBty = new byte[2];
                    for (int j = 0; j < 2; j++)
                    {
                        tmpBty[j] = buffer[i * 50 + 36 + j];
                    }
                    string tmpstart = BitConverter.ToString(tmpBty, 0).Replace("-", string.Empty).ToUpper();
                    if (tmpstart != "FCFC") 
                    {
                        break;
                    }
                    else
                    {
                        byte[] childBty = new byte[50];
                        for (int k = 0; k < 50; k++)
                        {
                            childBty[k] = buffer[i * 50 + 36 + k];
                        }
                        (buffer, childBty);
                    }
                }
            }
        }
    }
}
