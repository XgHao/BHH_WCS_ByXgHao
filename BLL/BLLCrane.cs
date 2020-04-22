using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseData;
using ComResolution;
using LOG;

namespace BLL
{
    /// <summary>
    /// 堆垛机业务逻辑层
    /// </summary>
    public class BLLCrane : BLLBase
    {
        Log logwrite = new Log("业务逻辑", @".\堆垛机日志\");

        /// <summary>
        /// 将异常信息投射到控件事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public delegate void Notify(string type, string msg);
        public event Notify NotifyEvent;

        public delegate void CraneDrawDg(List<CRCObject> lscro, List<CraneStr> lscrs);
        public event CraneDrawDg CraneDrawEvent;

        CRLCRANEControl crc = new CRLCRANEControl();

        public void Run()
        {
            crc.Run();
        }
    }
}
