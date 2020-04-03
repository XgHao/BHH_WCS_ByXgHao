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

        [DllImport("libnodave.dll")]
        private static extern int DaveConnectPLC(IntPtr dc);
        public int ConnectPL()
        {
            return DaveConnectPLC(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveDisconnectPLC(IntPtr dc);
        public int DisconnectPLC()
        {
            return DaveDisconnectPLC(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveReadBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
        public int ReadBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return DaveReadBytes(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveReadManyBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
        public int ReadManyBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return DaveReadManyBytes(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveReadBits(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
        public int ReadBits(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return DaveReadBits(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveWriteBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
        public int WriteBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return DaveWriteBytes(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveWriteManyBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
        public int WriteManyBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return DaveWriteManyBytes(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveWriteBits(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
        public int WriteBits(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return DaveWriteBits(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveGetS32(IntPtr dc);
        public int GetS32()
        {
            return DaveGetS32(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern uint DaveGetU32(IntPtr dc);
        public uint GetU32()
        {
            return DaveGetU32(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern short DaveGetS16(IntPtr dc);
        public short GetS16()
        {
            return DaveGetS16(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern ushort DaveGetU16(IntPtr dc);
        public ushort GetU16()
        {
            return DaveGetU16(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern sbyte DaveGetS8(IntPtr dc);
        public sbyte GetS8()
        {
            return DaveGetS8(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern byte DaveGetU8(IntPtr dc);
        public byte GetU8()
        {
            return DaveGetU8(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern float DaveGetFloat(IntPtr dc);
        public float GetFloat()
        {
            return DaveGetFloat(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveGetCounterValue(IntPtr dc);
        public int GetCounterValue()
        {
            return DaveGetCounterValue(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern float DaveGetSeconds(IntPtr dc);
        public float GetSeconds()
        {
            return DaveGetSeconds(Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveGetS32At(IntPtr dc, int pos);
        public int GetS32At(int pos)
        {
            return DaveGetS32At(Pointer, pos);
        }

        [DllImport("libnodave.dll")]
        private static extern uint DaveGetU32At(IntPtr dc, int pos);
        public uint GetU32At(int pos)
        {
            return DaveGetU32At(Pointer, pos);
        }

        [DllImport("libnodave.dll")]
        private static extern short DaveGetS16At(IntPtr dc, int pos);
        public short GetS16At(int pos)
        {
            return DaveGetS16At(Pointer, pos);
        }

        [DllImport("libnodave.dll")]
        private static extern ushort DaveGetU16At(IntPtr dc, int pos);
        public ushort GetU16At(int pos)
        {
            return DaveGetU16At(Pointer, pos);
        }

        [DllImport("libnodave.dll")]
        private static extern sbyte DaveGetS8At(IntPtr dc, int pos);
        public sbyte GetS8At(int pos)
        {
            return DaveGetS8At(Pointer, pos);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveGetU8At(IntPtr dc, int pos);
        public int GetU8At(int pos)
        {
            return DaveGetU8At(Pointer, pos);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern float DaveGetFloatAt(IntPtr dc, int pos);
        public float GetFloatAt(int pos)
        {
            return DaveGetFloatAt(Pointer, pos);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveGetCounterValueAt(IntPtr dc, int pos);
        public int GetCounterValueAt(int pos)
        {
            return DaveGetCounterValueAt(Pointer, pos);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern float DaveGetSecondsAt(IntPtr dc, int pos);
        public float GetSecondsAt(int pos)
        {
            return DaveGetSecondsAt(Pointer, pos);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveGetAnswLen(IntPtr dc);
        public int GetAnswLen()
        {
            return DaveGetAnswLen(Pointer);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveGetMaxPDULen(IntPtr dc);
        public int GetMaxPDULen()
        {
            return DaveGetMaxPDULen(Pointer);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DavePrepareReadRequest(IntPtr dc, IntPtr p);
        public ReadPDU PrepareReadRequest()
        {
            ReadPDU p = new ReadPDU();
            DavePrepareReadRequest(Pointer, p.Pointer);
            return p;
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DavePrepareWriteRequest(IntPtr dc, IntPtr p);
        public WritePDU PrepareWriteRequest()
        {
            WritePDU p = new WritePDU();
            DavePrepareWriteRequest(Pointer, p.Pointer);
            return p;
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveExecReadRequest(IntPtr dc, IntPtr p, IntPtr rl);
        public int ExecReadRequest(ReadPDU p, ResultSet rl)
        {
            return DaveExecReadRequest(Pointer, p.Pointer, rl.Pointer);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveExecWriteRequest(IntPtr dc, IntPtr p, IntPtr rl);
        public int ExecWriteRequest(WritePDU p, ResultSet rl)
        {
            return DaveExecWriteRequest(Pointer, p.Pointer, rl.Pointer);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveUseResult(IntPtr dc, IntPtr rs, int number);
        public int UseResult(ResultSet rs, int number)
        {
            return DaveUseResult(Pointer, rs.Pointer, number);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveReadSZL(IntPtr dc, int id, int index, byte[] ddd, int len);
        public int ReadSZL(int id, int index, byte[] ddd, int len)
        {
            return DaveReadSZL(Pointer, id, index, ddd, len);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveStart(IntPtr dc);
        public int Start()
        {
            return DaveStart(Pointer);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveStop(IntPtr dc);
        public int Stop()
        {
            return DaveStop(Pointer);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveForce200(IntPtr dc, int area, int start, int val);
        public int Force200(PLCEnum.PlcMemoryArea area, int start, int val)
        {
            return DaveForce200(Pointer, (int)area, start, val);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveForceDisconnectIBH(IntPtr dc, int src, int dest, int MPI);
        public int ForceDisconnectIBH(int src, int dest, int MPI)
        {
            return DaveForceDisconnectIBH(Pointer, src, dest, MPI);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveGetResponse(IntPtr dc);
        public int GetResponse()
        {
            return DaveGetResponse(Pointer);
        }

        [DllImport("libnodave.dll"/*, PreserveSig=false */ )]
        private static extern int DaveSendMessage(IntPtr dc, IntPtr p);
        public int SendMessage(PDU p)
        {
            return DaveSendMessage(Pointer, p.Pointer);
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveGetProgramBlock(IntPtr dc, int blockType, int number, byte[] buffer, ref int length);
        public int GetProgramBlock(PLCEnum.S7BlockType blockType, int number, byte[] buffer, ref int length)
        {
            Console.WriteLine("length:" + length);
            int a = DaveGetProgramBlock(Pointer, (int)blockType, number, buffer, ref length);
            Console.WriteLine("length:" + length);
            return a;
        }

        [DllImport("libnodave.dll")]
        private static extern int DaveListBlocksOfType(IntPtr dc, int blockType, byte[] buffer);
        public int ListBlocksOfType(PLCEnum.S7BlockType blockType, byte[] buffer)
        {
            return DaveListBlocksOfType(Pointer, (int)blockType, buffer);
        }
    }
}
