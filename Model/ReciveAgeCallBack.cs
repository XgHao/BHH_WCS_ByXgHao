using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class ReciveAgeCallBack
    {
        [DataMember]
        public string ReqCode { get; set; }

        [DataMember]
        public string ReqTime { get; set; }

        [DataMember]
        public string InterfaceName { get; set; }

        [DataMember]
        public string CooX { get; set; }

        [DataMember]
        public string CooY { get; set; }

        [DataMember]
        public string CurrentPositionCode { get; set; }

        [DataMember]
        public string Data { get; set; }

        [DataMember]
        public string MapCode { get; set; }

        [DataMember]
        public string MapDataCode { get; set; }

        [DataMember]
        public string Method { get; set; }

        [DataMember]
        public string PodCode { get; set; }

        [DataMember]
        public string PodDir { get; set; }

        [DataMember]
        public string RobotCode { get; set; }

        [DataMember]
        public string TaskCode { get; set; }

        [DataMember]
        public string WbCode { get; set; }
    }
}
