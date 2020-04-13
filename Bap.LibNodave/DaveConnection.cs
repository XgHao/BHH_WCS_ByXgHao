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
        public DaveConnection(DaveInterface di, int MPI, int rack, int slot)
        {
            Pointer = API.DaveNewConnection(di.Pointer, MPI, rack, slot);
        }

        public int ConnectPL()
        {
            return API.DaveConnectPLC(Pointer);
        }

        public int DisconnectPLC()
        {
            return API.DaveDisconnectPLC(Pointer);
        }

        public int ReadBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return API.DaveReadBytes(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        public int ReadManyBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return API.DaveReadManyBytes(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        public int ReadBits(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return API.DaveReadBits(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        public int WriteBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return API.DaveWriteBytes(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        public int WriteManyBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return API.DaveWriteManyBytes(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        public int WriteBits(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return API.DaveWriteBits(Pointer, (int)area, DBnumber, start, len, buffer);
        }

        public int GetS32()
        {
            return API.DaveGetS32(Pointer);
        }

        public uint GetU32()
        {
            return API.DaveGetU32(Pointer);
        }

        public short GetS16()
        {
            return API.DaveGetS16(Pointer);
        }

        public ushort GetU16()
        {
            return API.DaveGetU16(Pointer);
        }

        public sbyte GetS8()
        {
            return API.DaveGetS8(Pointer);
        }

        public byte GetU8()
        {
            return API.DaveGetU8(Pointer);
        }

        public float GetFloat()
        {
            return API.DaveGetFloat(Pointer);
        }

        public int GetCounterValue()
        {
            return API.DaveGetCounterValue(Pointer);
        }

        public float GetSeconds()
        {
            return API.DaveGetSeconds(Pointer);
        }

        public int GetS32At(int pos)
        {
            return API.DaveGetS32At(Pointer, pos);
        }

        public uint GetU32At(int pos)
        {
            return API.DaveGetU32At(Pointer, pos);
        }

        public short GetS16At(int pos)
        {
            return API.DaveGetS16At(Pointer, pos);
        }

        public ushort GetU16At(int pos)
        {
            return API.DaveGetU16At(Pointer, pos);
        }

        public sbyte GetS8At(int pos)
        {
            return API.DaveGetS8At(Pointer, pos);
        }

        public byte GetU8At(int pos)
        {
            return API.DaveGetU8At(Pointer, pos);
        }

        public float GetFloatAt(int pos)
        {
            return API.DaveGetFloatAt(Pointer, pos);
        }

        public int GetCounterValueAt(int pos)
        {
            return API.DaveGetCounterValueAt(Pointer, pos);
        }

        public float GetSecondsAt(int pos)
        {
            return API.DaveGetSecondsAt(Pointer, pos);
        }

        public int GetAnswLen()
        {
            return API.DaveGetAnswLen(Pointer);
        }

        public int GetMaxPDULen()
        {
            return API.DaveGetMaxPDULen(Pointer);
        }

        public ReadPDU PrepareReadRequest()
        {
            ReadPDU p = new ReadPDU();
            API.DavePrepareReadRequest(Pointer, p.Pointer);
            return p;
        }

        public WritePDU PrepareWriteRequest()
        {
            WritePDU p = new WritePDU();
            API.DavePrepareWriteRequest(Pointer, p.Pointer);
            return p;
        }

        public int ExecReadRequest(ReadPDU p, ResultSet rl)
        {
            return API.DaveExecReadRequest(Pointer, p.Pointer, rl.Pointer);
        }

        public int ExecWriteRequest(WritePDU p, ResultSet rl)
        {
            return API.DaveExecWriteRequest(Pointer, p.Pointer, rl.Pointer);
        }

        public int UseResult(ResultSet rs, int number)
        {
            return API.DaveUseResult(Pointer, rs.Pointer, number);
        }

        public int ReadSZL(int id, int index, byte[] ddd, int len)
        {
            return API.DaveReadSZL(Pointer, id, index, ddd, len);
        }

        public int Start()
        {
            return API.DaveStart(Pointer);
        }

        public int Stop()
        {
            return API.DaveStop(Pointer);
        }

        public int Force200(PLCEnum.PlcMemoryArea area, int start, int val)
        {
            return API.DaveForce200(Pointer, (int)area, start, val);
        }

        public int ForceDisconnectIBH(int src, int dest, int MPI)
        {
            return API.DaveForceDisconnectIBH(Pointer, src, dest, MPI);
        }

        public int GetResponse()
        {
            return API.DaveGetResponse(Pointer);
        }

        public int SendMessage(PDU p)
        {
            return API.DaveSendMessage(Pointer, p.Pointer);
        }

        public int GetProgramBlock(PLCEnum.S7BlockType blockType, int number, byte[] buffer, ref int length)
        {
            Console.WriteLine("length:" + length);
            int a = API.DaveGetProgramBlock(Pointer, (int)blockType, number, buffer, ref length);
            Console.WriteLine("length:" + length);
            return a;
        }

        public int ListBlocksOfType(PLCEnum.S7BlockType blockType, byte[] buffer)
        {
            return API.DaveListBlocksOfType(Pointer, (int)blockType, buffer);
        }
    }
}
