using BaseData;
using LOG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComResolution
{
    public class CRLCRANEControl : CRLBase
    {
        SRMStr srs = new SRMStr();
        public static Log logwrite = new Log("系统日志", @".\堆垛机日志\");

        /// <summary>
        /// 将异常内容反馈给窗体
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public delegate void NotifyShow(string type, string msg);
        public event NotifyShow NotifyShowEvent;

        public delegate void IssuedCraneTaskDG(string craneid, byte[] buffer);
        public event IssuedCraneTaskDG IssuedCraneTaskEvent;

        public delegate void CraneDrawDg(List<CRCObject> cro, List<CraneStr> crs);
        public event CraneDrawDg CraneDrawEvent;

        public Task task;
        public void Run()
        {
            #region 堆垛机基础信息

            #endregion

            foreach (var co in CrcList)
            {
                //连接成功 则读取堆垛机内信息
                co.BllSrm.NotifyShowEvent += ShowText;
                co.BllSrm.Run();
                task = Task.Factory.StartNew(TimeScan, co);
            }
        }

        private void TimeScan(object ob)
        {
            while (true)
            {
                CRCObject co = ob as CRCObject;
                co.BllSrm.CraneRead(ref co);
                Thread.Sleep(500);
            }
        }

        private void ShowText(string type, string msg)
        {
            throw new NotImplementedException();
        }
    }
}
