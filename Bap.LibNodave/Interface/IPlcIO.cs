using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bap.LibNodave.Interface
{
    interface IPlcIO : IDisposable
    {
        int ConnectPLC();

        void DisConnectPLC();

        int ExecReadRequest(ReadPDU p,res);
    }
}
