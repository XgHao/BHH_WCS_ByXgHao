using Bap.LibNodave.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bap.LibNodave
{
    public class DaveInterface : PseudoPointer
    {
        public DaveInterface(FD fd, string name, int localMPI, PLCEnum.Protocols useProto, PLCEnum.ProfiBusSpeed speed)
        {
            Pointer = API.DaveNewInterface(fd, name, localMPI, (int)useProto, (int)speed);
        }

        public int InitAdapter()
        {
            return API.DaveInitAdapter(Pointer);
        }

        public int ListReachablePartners(byte[] buffer)
        {
            return API.DaveListReachablePartners(Pointer, buffer);
        }

        public int Timeout
        {
            get { return API.DaveGetTimeout(Pointer); }
            set { API.DaveSetTimeout(Pointer, value); }
        }

        public IntPtr DisconnectAdapter()
        {
            return API.DaveDisconnectAdapter(Pointer);
        }

        public string Name
        {
            get { return API.DaveGetName(Pointer); }
        }
    }
}
