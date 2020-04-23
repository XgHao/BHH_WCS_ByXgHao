using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class ContinueToAgvInfo
    {
        [DataMember]
        public string ReqCode { get; set; }

        [DataMember]
        public string ReqTime { get; set; }

        [DataMember]
        public string ClientCode { get; set; }

        [DataMember]
        public string TokenCode { get; set; }

        [DataMember]
        public string InterFaceName { get; set; }

        [DataMember]
        public string WbCode { get; set; }

        [DataMember]
        public string PodCode { get; set; }

        [DataMember]
        public string AgvCode { get; set; }

        [DataMember]
        public string TaskCode { get; set; }

        [DataMember]
        public string TaskSeq { get; set; }

        [DataMember]
        public string Data { get; set; }

        [DataMember]
        public Positioncodepath NextPositionCode { get; set; }
    }
}
