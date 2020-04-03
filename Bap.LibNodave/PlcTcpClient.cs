using Bap.LibNodave.Interface;
using Bap.LibNodave.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Bap.LibNodave
{
    public class PlcTcpClient : IPlcIO
    {
        private FD mFds;
        private DaveInterface mDI = null;
        private DaveConnection mConn = null;
        private Socket cliSocket;

        public string IpAddress { get; private set; }

        public bool IsConnected { get; private set; }

        public string Name { get; private set; }

        public int PlcRack { get; private set; }

        public int PlcSlot { get; private set; }

        public int TcpPort { get; private set; }

        public PlcTcpClient(string ip, int port, int rack = 0, int slot = 1, string name = "PlcTcpClient")
        {
            IpAddress = ip;
            TcpPort = port;
            PlcRack = rack;
            PlcSlot = slot;
            Name = name;
            IsConnected = false;
        }

        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <returns></returns>
        public int ConnectPLC()
        {
            if (IsConnected)
                return 0;

            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(IpAddress), TcpPort);
            timeoutSocket
        }

        public void DisConnectPLC()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int ExecReadRequest(ReadPDU p, ResultSet rl)
        {
            throw new NotImplementedException();
        }

        public int ExecWriteRequest(WritePDU p, ResultSet rl)
        {
            throw new NotImplementedException();
        }

        public int Force200(PLCEnum.PlcMemoryArea area, int start, int val)
        {
            throw new NotImplementedException();
        }

        public int ForceDisconnectIBH(int src, int dest, int MPI)
        {
            throw new NotImplementedException();
        }

        public int GetAnswLen()
        {
            throw new NotImplementedException();
        }

        public int GetCounterValue()
        {
            throw new NotImplementedException();
        }

        public int GetCounterValueAt(int pos)
        {
            throw new NotImplementedException();
        }

        public float GetFloat()
        {
            throw new NotImplementedException();
        }

        public float GetFloatAt(int pos)
        {
            throw new NotImplementedException();
        }

        public int GetMaxPDULen()
        {
            throw new NotImplementedException();
        }

        public int GetProgramBlock(PLCEnum.S7BlockType blockType, int number, byte[] buffer, ref int length)
        {
            throw new NotImplementedException();
        }

        public int GetResponse()
        {
            throw new NotImplementedException();
        }

        public short GetS16()
        {
            throw new NotImplementedException();
        }

        public short GetS16At(int pos)
        {
            throw new NotImplementedException();
        }

        public int GetS32()
        {
            throw new NotImplementedException();
        }

        public int GetS32At(int pos)
        {
            throw new NotImplementedException();
        }

        public sbyte GetS8()
        {
            throw new NotImplementedException();
        }

        public sbyte GetS8At(int pos)
        {
            throw new NotImplementedException();
        }

        public float GetSeconds()
        {
            throw new NotImplementedException();
        }

        public float GetSecondsAt(int pos)
        {
            throw new NotImplementedException();
        }

        public ushort GetU16()
        {
            throw new NotImplementedException();
        }

        public ushort GetU16At(int pos)
        {
            throw new NotImplementedException();
        }

        public uint GetU32()
        {
            throw new NotImplementedException();
        }

        public uint GetU32At(int pos)
        {
            throw new NotImplementedException();
        }

        public byte GetU8()
        {
            throw new NotImplementedException();
        }

        public byte GetU8At(int pos)
        {
            throw new NotImplementedException();
        }

        public int ListBlocksOfType(PLCEnum.S7BlockType blockType, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public ReadPDU PrepareReadRequest()
        {
            throw new NotImplementedException();
        }

        public WritePDU PrepareWriteRequest()
        {
            throw new NotImplementedException();
        }

        public int ReadBits(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int ReadBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int ReadManyBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int ReadSZL(int id, int index, byte[] ddd, int len)
        {
            throw new NotImplementedException();
        }

        public int SendMessage(PDU p)
        {
            throw new NotImplementedException();
        }

        public int Start()
        {
            throw new NotImplementedException();
        }

        public int Stop()
        {
            throw new NotImplementedException();
        }

        public int UseResult(ResultSet rs, int number)
        {
            throw new NotImplementedException();
        }

        public int WriteBits(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int WriteBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int WriteManyBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}
