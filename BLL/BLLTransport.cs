using BaseData;
using ComResolution;
using LedDisplay;
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
    public class BLLTransport : BLLBase
    {
        public static Log logwrite = new Log("业务逻辑", @".\输送机日志\");
        CRLTRANSControl crlTrsnsClass = new CRLTRANSControl();
        public delegate void NotifyDg(string msg);
        public event NotifyDg NotifyEvent;

        public delegate void GetEquipMentIndoDg(List<TransportStr> transportStrs);
        public event GetEquipMentIndoDg EquipMentInfoEvent;
        /// <summary>
        /// 重新初始化输送机
        /// </summary>
        private Timer CheckTimer = null;
        public Timer CheckInitTimer
        {
            get
            {
                if (CheckTimer == null)
                {
                    CheckTimer = new Timer();
                }
                return CheckTimer;
            }
        }

        LedServer led = new LedServer();

        public void run()
        {
            crltranclass.GetEquipMentInfoEvent += transopoftstr => EquipMentInfoEvent?.Invoke(transopoftstr);
            crltranclass.NotifyEvent += msg => NotifyEvent?.Invoke(msg);
            CheckTimer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["CheckConnectTimer"]);
            CheckTimer.Elapsed += (sender, e) =>
            {
                if (crltranclass.TransDeviceInit())
                {
                    NotifyEvent.Invoke("输送机重新初始化成功");
                    CheckTimer.Stop();
                }
                else
                {
                    NotifyEvent.Invoke("输送机重新初始化失败");
                }
            };

            if (crltranclass.TransDeviceInit())
            {
                NotifyEvent.Invoke("输送机初始化成功");
                crltranclass.Run();
            }
            else
            {
                NotifyEvent.Invoke("输送机初始化失败");
                CheckTimer.Start();
            }
        }

    }
}
