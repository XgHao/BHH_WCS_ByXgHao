using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bap.LibNodave
{
    public class DaveConnection : PseudoPointer
    {
        [DllImport("libnodave.dll")]
        private static extern IntPtr DaveNewConnection(IntPtr di, int MPI, int rack, int slot);
        public DaveConnection(DaveInterface di, int MPI, int rack, int slot)
        {
            Pointer = DaveNewConnection(di.Pointer, MPI, rack, slot);
        }
    }
}
