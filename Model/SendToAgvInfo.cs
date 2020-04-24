using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [DataContract]
    public class SendToAgvInfo
    {
        [DataMember]
        public string ReqCode { get; set; }

        [DataMember]
        public string ReqTime { get; set; }

        [DataMember]
        public string ClinetCode { get; set; }

        [DataMember]
        public string TokenCode { get; set; }

        [DataMember]
        public string InterFaceName { get; set; }

        [DataMember]
        public string TaskType { get; set; }

        [DataMember]
        public string WbCode { get; set; }

        [DataMember]
        public string PodCode { get; set; }

        [DataMember]
        public string PodDir { get; set; }

        [DataMember]
        public string Priority { get; set; }

        [DataMember]
        public string AgvCode { get; set; }

        [DataMember]
        public string TaskCode { get; set; }

        [DataMember]
        public string Data { get; set; }

        [DataMember]
        public Positioncodepath[] PositionCodePaths { get; set; }
    }
}
