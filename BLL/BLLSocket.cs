using BaseData;
using ComResolution;
using LOG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    /// <summary>
    /// 自动收获Socket通信方式
    /// </summary>
    public class BLLSocket : BLLBase
    {
        /// <summary>
        /// 将异常信息投射到控件事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public delegate void Notify(string type, string msg);
        public event Notify NotifyEvent;

        CRLSocketControl crl = new CRLSocketControl();
        RECDETAIL recedTail = new RECDETAIL();
        Log logWrite = new Log("Socket日志", @".\socket业务逻辑\");

        public void Run()
        {
            crl.NotifyShowEvent += (type, msg) => NotifyEvent?.Invoke(type, msg);
            crl.Run();
        }
    }
}
