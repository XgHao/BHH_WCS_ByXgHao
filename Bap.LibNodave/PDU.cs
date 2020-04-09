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
        [DllImport("libnodave.dll", EntryPoint = "daveNewPDU")]
        private static extern IntPtr DaveNewPDU();

        public PDU()
        {
            Pointer = DaveNewPDU();
        }
    }

    public class ReadPDU : PDU
    {
        [DllImport("libnodave.dll", EntryPoint = "daveAddVarToReadRequest")]
        private static extern void DaveAddVarToReadRequest(IntPtr p, int area, int DBnum, int start, int bytes);
        public void AddVarToReadRequest(PLCEnum.PlcMemoryArea area,int DBum,int start,int bytes)
        {
            DaveAddVarToReadRequest(Pointer, (int)area, DBum, start, bytes);
        }

        [DllImport("libnodave.dll", EntryPoint = "daveAddBitVarToReadRequest")]
        private static extern void DaveAddBitVarToReadRequest(IntPtr p, int area, int DBnum, int start, int bytes);
        public void AddBitVarToReadRequest(PLCEnum.PlcMemoryArea area, int DBum, int start, int bytes)
        {
            DaveAddBitVarToReadRequest(Pointer, (int)area, DBum, start, bytes);
        }
    }

    public class WritePDU : PDU
    {
        [DllImport("libnodave.dll", EntryPoint = "daveAddVarToWriteRequest")]
        private static extern void DaveAddVarToWriteRequest(IntPtr p, int area, int DBnum, int start, int bytes, byte[] buffer);
        public void AddVarToWriteRequest(PLCEnum.PlcMemoryArea area, int DBum, int start, int bytes, byte[] buffer)
        {
            DaveAddVarToWriteRequest(Pointer, (int)area, DBum, start, bytes, buffer);
        }

        [DllImport("libnodave.dll", EntryPoint = "daveAddBitVarToWriteRequest")]
        private static extern void DaveAddBitVarToWriteRequest(IntPtr p, int area, int DBnum, int start, int bytes, byte[] buffer);
        public void AddBitVarToWriteRequest(PLCEnum.PlcMemoryArea area, int DBum, int start, int bytes, byte[] buffer)
        {
            DaveAddBitVarToWriteRequest(Pointer, (int)area, DBum, start, bytes, buffer);
        }
    }
}
