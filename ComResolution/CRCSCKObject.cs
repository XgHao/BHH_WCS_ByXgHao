using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComResolution
{
    public class CRCSCKObject
    {
        public string SCName { get; set; }
        public CRLSocket CRCSck { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int AisleNumber { get; set; }
        public int ConnectStatus { get; set; }
        public string EquipmentId { get; set; }
    }
}
