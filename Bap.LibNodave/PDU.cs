using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
 
namespace Bap.LibNodave
{
    /// <summary>
    /// 协议数据单元（Protocol Data Unit）
    /// </summary>
    public class PDU : PseudoPointer
    {
        public PDU()
        {
            Pointer = API.DaveNewPDU();
        }
    }

    public class ReadPDU : PDU
    {
        public void AddVarToReadRequest(PLCEnum.PlcMemoryArea area,int DBum,int start,int bytes)
        {
            API.DaveAddVarToReadRequest(Pointer, (int)area, DBum, start, bytes);
        }

        public void AddBitVarToReadRequest(PLCEnum.PlcMemoryArea area, int DBum, int start, int bytes)
        {
            API.DaveAddBitVarToReadRequest(Pointer, (int)area, DBum, start, bytes);
        }
    }

    public class WritePDU : PDU
    {
        public void AddVarToWriteRequest(PLCEnum.PlcMemoryArea area, int DBum, int start, int bytes, byte[] buffer)
        {
            API.DaveAddVarToWriteRequest(Pointer, (int)area, DBum, start, bytes, buffer);
        }

        public void AddBitVarToWriteRequest(PLCEnum.PlcMemoryArea area, int DBum, int start, int bytes, byte[] buffer)
        {
            API.DaveAddBitVarToWriteRequest(Pointer, (int)area, DBum, start, bytes, buffer);
        }
    }
}
