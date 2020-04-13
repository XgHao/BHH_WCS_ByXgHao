using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComResolution
{
    /// <summary>
    /// 堆垛机实体类
    /// </summary>
    public class CRCObject
    {
        public string ScNo { get; set; }
        public CRLCRANEConnect BllSrm { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int aislenumber { get; set; }
        public int connectstatus { get; set; }
        public string equipmentid { get; set; }
        public int runorder { get; set; }
        public int ordertmp { get; set; }
    }
}
