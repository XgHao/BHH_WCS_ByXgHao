using BaseData;
using ComResolution;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BLL
{
    public class BLLSRMEQUIP : BLLBase
    {
        public delegate void Notify(string type, string msg);
        /// <summary>
        /// 将异常信息投射到控件事件
        /// </summary>
        public event Notify NotifyEvent;

        public BLLSRMEQUIP()
        {

        }

        public void Run()
        {
            #region 堆垛机基础信息
            XmlDocument doc = new XmlDocument();
            doc.Load(@"..\..\XmlSRM.xml");

            XmlNode xn = doc.SelectSingleNode("SRM");

            XmlNodeList xnl = xn.ChildNodes;
            foreach (var item in xnl)
            {
                XmlElement xe = item as XmlElement;
                XmlNodeList xnll = xe.ChildNodes;
                SRMStr sr = new SRMStr
                {
                    Ip = xnll.Item(0).InnerText,
                    Name = xnll.Item(1).InnerText,
                    Port = xnll.Item(2).InnerText
                };
                BLLSRMERegist lsr = new BLLSRMERegist(sr);
                CRCObject newcrc = new CRCObject
                {
                    //BllSrm = lsr,
                    IpAddress = sr.Ip,
                    Port = int.Parse(sr.Port),
                    ScNo = sr.Name
                };
            }
            #endregion
        }
    }
}
