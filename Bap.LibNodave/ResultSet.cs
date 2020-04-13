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
        public ResultSet()
        {
            Pointer = API.DaveNewResultSet();
        }

        public int GetErrorOfResult(int number)
        {
            return API.DaveGetErrorOfResult(Pointer, number);
        }
    }
}
