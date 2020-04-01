using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bap.LibNodave
{
    public class ResultSet : PseudoPointer
    {
        [DllImport("libnodave.dll", EntryPoint = "DaveNewResultSet")]
        private static extern IntPtr DaveNewResultSet();
        public ResultSet()
        {
            Pointer = DaveNewResultSet();
        }

        [DllImport("libnodave.dll", EntryPoint = "DaveFreeResults")]
        private static extern void DaveFreeResults(IntPtr rs);

        [DllImport("libnodave.dll", EntryPoint = "DaveGetErrorOfResult")]
        private static extern int DaveGetErrorOfResult(IntPtr rs, int number);
        public int GetErrorOfResult(int number)
        {
            return DaveGetErrorOfResult(Pointer, number);
        }
    }
}
