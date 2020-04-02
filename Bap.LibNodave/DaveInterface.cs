using Bap.LibNodave.Properties;
using Bap.LibNodave.Struct;
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
        [DllImport("libnodave.dll")]
        private static extern IntPtr DaveNewInterface(
                [MarshalAs(UnmanagedType.Struct)] FD fd,
                [MarshalAs(UnmanagedType.LPWStr)] string name,
                int localMPI,
                int useProto,
                int speed
            );

        public DaveInterface(FD fd, string name, int localMPI, PLCEnum.Protocols useProto, PLCEnum.ProfiBusSpeed speed)
        {
            Pointer = DaveNewInterface(fd, name, localMPI, (int)useProto, (int)speed);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveInitAdapter(IntPtr di);
        public int InitAdapter()
        {
            return DaveInitAdapter(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveListReachablePartners(IntPtr di, byte[] buffer);
        public int ListReachablePartners(byte[] buffer)
        {
            return DaveListReachablePartners(Pointer, buffer);
        }

        [DllImport("libnodave.dll")]
        private static extern void DaveSetTimeout(IntPtr di, int time);
        [DllImport("libnodave.dll")]
        private static extern int DaveGetTimeout(IntPtr di);
        public int Timeout
        {
            get { return DaveGetTimeout(Pointer); }
            set { DaveSetTimeout(Pointer, value); }
        }

        [DllImport("libnodave.dll")]
        private static extern IntPtr DaveDisconnectAdapter(IntPtr di);
        public IntPtr DisconnectAdapter()
        {
            return DaveDisconnectAdapter(Pointer);
        }

        [DllImport("libnodave.dll", CharSet = CharSet.Unicode)]
        private static extern string DaveGetName(IntPtr di);
        public string Name
        {
            get { return DaveGetName(Pointer); }
        }
    }
}
