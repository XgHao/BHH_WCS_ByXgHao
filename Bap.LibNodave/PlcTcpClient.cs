using Bap.LibNodave.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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

        bool disposed = false;

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
            TimeOutSocket timeOut = new TimeOutSocket();
            cliSocket = timeOut.Connect(ipe, 300);
            if (cliSocket != null) 
            {
                mFds.rfd = API.OpenSocket(TcpPort, IpAddress);
                if (mFds.rfd <= 0)
                {
                    //FD一般是不可能为0的，0、1、2、3、4等估计早被操作系统或者其他程序占用了
                    return mFds.rfd;
                }

                mFds.wfd = mFds.rfd;
                mDI = new DaveInterface(mFds, "PLC_1", 0, PLCEnum.Protocols.ISOTCP, PLCEnum.ProfiBusSpeed.Speed187k);
                mDI.Timeout = 1;
                mConn = new DaveConnection(mDI, 0, PlcRack, PlcSlot);
                int rc = mConn.ConnectPL();
                if (rc == 0)
                {
                    IsConnected = true;
                    return rc;
                }
                return rc;
            }
            return -1;
        }

        public void DisConnectPLC()
        {
            if (!IsConnected)
                return;

            if (mConn != null) 
            {
                mConn.DisconnectPLC();
                mConn.Dispose();
            }

            if (mDI != null) 
            {
                mDI.Dispose();
            }

            API.CloseSocket(mFds.rfd);
            mConn = null;
            mDI = null;
            IsConnected = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                DisConnectPLC();
            }

            disposed = true;
        }

        ~PlcTcpClient()
        {
            Dispose(false);
        }

        public int ExecReadRequest(ReadPDU p, ResultSet rl)
        {
            return mConn.ExecReadRequest(p, rl);
        }

        public int ExecWriteRequest(WritePDU p, ResultSet rl)
        {
            return mConn.ExecWriteRequest(p, rl);
        }

        public int Force200(PLCEnum.PlcMemoryArea area, int start, int val)
        {
            return mConn.Force200(area, start, val);
        }

        public int ForceDisconnectIBH(int src, int dest, int MPI)
        {
            return mConn.ForceDisconnectIBH(src, dest, MPI);
        }

        public int GetAnswLen()
        {
            return mConn.GetAnswLen();
        }

        public int GetCounterValue()
        {
            return mConn.GetCounterValue();
        }

        public int GetCounterValueAt(int pos)
        {
            return mConn.GetCounterValueAt(pos);
        }

        public float GetFloat()
        {
            return mConn.GetFloat();
        }

        public float GetFloatAt(int pos)
        {
            return mConn.GetFloatAt(pos);
        }

        public int GetMaxPDULen()
        {
            return mConn.GetMaxPDULen();
        }

        public int GetProgramBlock(PLCEnum.S7BlockType blockType, int number, byte[] buffer, ref int length)
        {
            return mConn.GetProgramBlock(blockType, number, buffer, ref length);
        }

        public int GetResponse()
        {
            return mConn.GetResponse();
        }

        /// <summary>
        /// short
        /// </summary>
        /// <returns></returns>
        public short GetS16()
        {
            return mConn.GetS16();
        }

        public short GetS16At(int pos)
        {
            return mConn.GetS16At(pos);
        }

        /// <summary>
        /// int
        /// </summary>
        /// <returns></returns>
        public int GetS32()
        {
            return mConn.GetS32();
        }

        public int GetS32At(int pos)
        {
            return mConn.GetS32At(pos);
        }

        /// <summary>
        /// sbyte
        /// </summary>
        /// <returns></returns>
        public sbyte GetS8()
        {
            return mConn.GetS8();
        }

        public sbyte GetS8At(int pos)
        {
            return mConn.GetS8At(pos);
        }

        public float GetSeconds()
        {
            return mConn.GetSeconds();
        }

        public float GetSecondsAt(int pos)
        {
            return mConn.GetSecondsAt(pos);
        }

        /// <summary>
        /// ushort
        /// </summary>
        /// <returns></returns>
        public ushort GetU16()
        {
            return mConn.GetU16();
        }

        public ushort GetU16At(int pos)
        {
            return mConn.GetU16At(pos);
        }

        /// <summary>
        /// uint
        /// </summary>
        /// <returns></returns>
        public uint GetU32()
        {
            return mConn.GetU32();
        }

        public uint GetU32At(int pos)
        {
            return mConn.GetU32At(pos);
        }

        /// <summary>
        /// byte
        /// </summary>
        /// <returns></returns>
        public byte GetU8()
        {
            return mConn.GetU8();
        }

        public byte GetU8At(int pos)
        {
            return mConn.GetU8At(pos);
        }

        public int ListBlocksOfType(PLCEnum.S7BlockType blockType, byte[] buffer)
        {
            return mConn.ListBlocksOfType(blockType, buffer);
        }

        public ReadPDU PrepareReadRequest()
        {
            return mConn.PrepareReadRequest();
        }

        public WritePDU PrepareWriteRequest()
        {
            return mConn.PrepareWriteRequest();
        }

        public int ReadBits(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return mConn.ReadBits(area, DBnumber, start, len, buffer);
        }

        public int ReadBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return mConn.ReadBytes(area, DBnumber, start, len, buffer);
        }

        public int ReadManyBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            lock (this)
            {
                return mConn.ReadManyBytes(area, DBnumber, start, len, buffer);
            }
        }

        public int ReadSZL(int id, int index, byte[] ddd, int len)
        {
            return mConn.ReadSZL(id, index, ddd, len);
        }

        public int SendMessage(PDU p)
        {
            return mConn.SendMessage(p);
        }

        public int Start()
        {
            return mConn.Start();
        }

        public int Stop()
        {
            return mConn.Stop();
        }

        public int UseResult(ResultSet rs, int number)
        {
            return mConn.UseResult(rs, number);
        }

        public int WriteBits(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return mConn.WriteBits(area, DBnumber, start, len, buffer);
        }

        public int WriteBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            return mConn.WriteBytes(area, DBnumber, start, len, buffer);
        }

        public int WriteManyBytes(PLCEnum.PlcMemoryArea area, int DBnumber, int start, int len, byte[] buffer)
        {
            lock (this)
            {
                return mConn.WriteManyBytes(area, DBnumber, start, len, buffer);
            }
        }

    }
}
