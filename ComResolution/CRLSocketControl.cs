using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComResolution
{
    public class CRLSocketControl : CRLBase
    {
        public delegate void RcvStrDG(object state);
        public event RcvStrDG RcvStrEvent;

        public delegate void NotifyShow(string type, string msg);
        /// <summary>
        /// 将异常内容反馈给窗体
        /// </summary>
        public event NotifyShow NotifyShowEvent;

        public void Run()
        {
            CRLSocket crs = new CRLSocket();
            crs.NotifyShowEvent += (type, msg) => NotifyShowEvent?.Invoke(type, msg);
            crs.RcvStrEvent += state => RcvStrEvent?.Invoke(state);
        }
    }
}
