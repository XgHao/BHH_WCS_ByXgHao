using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bap.LibNodave
{
    internal static class API
    {
        #region 打开或关闭Sokcet

        [DllImport("libnodave.dll", EntryPoint = "closeSocket")]
        internal static extern int CloseSocket(int port);

        [DllImport("libnodave.dll", EntryPoint = "openSocket")]
        internal static extern int OpenSocket(int port, [MarshalAs(UnmanagedType.LPWStr)]string ipAddress);

        #endregion

        #region DaveConnection
        [DllImport("libnodave.dll", EntryPoint = "daveNewConnection")]
        internal static extern IntPtr DaveNewConnection(IntPtr di, int MPI, int rack, int slot);
        
        [DllImport("libnodave.dll", EntryPoint = "daveConnectPLC")]
        internal static extern int DaveConnectPLC(IntPtr dc);

        [DllImport("libnodave.dll", EntryPoint = "daveDisconnectPLC")]
        internal static extern int DaveDisconnectPLC(IntPtr dc);

        [DllImport("libnodave.dll", EntryPoint = "daveReadBytes")]
        internal static extern int DaveReadBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
        
        [DllImport("libnodave.dll", EntryPoint = "daveReadManyBytes")]
        internal static extern int DaveReadManyBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
        
        [DllImport("libnodave.dll", EntryPoint = "daveReadBits")]
        internal static extern int DaveReadBits(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);

        [DllImport("libnodave.dll", EntryPoint = "daveWriteBytes")]
        internal static extern int DaveWriteBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);

        [DllImport("libnodave.dll", EntryPoint = "daveWriteManyBytes")]
        internal static extern int DaveWriteManyBytes(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
        
        [DllImport("libnodave.dll", EntryPoint = "daveWriteBits")]
        internal static extern int DaveWriteBits(IntPtr dc, int area, int DBnumber, int start, int len, byte[] buffer);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetS32")]
        internal static extern int DaveGetS32(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetU32")]
        internal static extern uint DaveGetU32(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetS16")]
        internal static extern short DaveGetS16(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetU16")]
        internal static extern ushort DaveGetU16(IntPtr dc);
       
        [DllImport("libnodave.dll", EntryPoint = "daveGetS8")]
        internal static extern sbyte DaveGetS8(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetU8")]
        internal static extern byte DaveGetU8(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetFloat")]
        internal static extern float DaveGetFloat(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetCounterValue")]
        internal static extern int DaveGetCounterValue(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetSeconds")]
        internal static extern float DaveGetSeconds(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetS32At")]
        internal static extern int DaveGetS32At(IntPtr dc, int pos);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetU32At")]
        internal static extern uint DaveGetU32At(IntPtr dc, int pos);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetS16At")]
        internal static extern short DaveGetS16At(IntPtr dc, int pos);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetU16At")]
        internal static extern ushort DaveGetU16At(IntPtr dc, int pos);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetS8At")]
        internal static extern sbyte DaveGetS8At(IntPtr dc, int pos);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetU8At")]
        internal static extern byte DaveGetU8At(IntPtr dc, int pos);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetFloatAt")]
        internal static extern float DaveGetFloatAt(IntPtr dc, int pos);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetCounterValueAt")]
        internal static extern int DaveGetCounterValueAt(IntPtr dc, int pos);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetSecondsAt")]
        internal static extern float DaveGetSecondsAt(IntPtr dc, int pos);
       
        [DllImport("libnodave.dll", EntryPoint = "daveGetAnswLen")]
        internal static extern int DaveGetAnswLen(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetMaxPDULen")]
        internal static extern int DaveGetMaxPDULen(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "davePrepareReadRequest")]
        internal static extern int DavePrepareReadRequest(IntPtr dc, IntPtr p);
        
        [DllImport("libnodave.dll", EntryPoint = "davePrepareWriteRequest")]
        internal static extern int DavePrepareWriteRequest(IntPtr dc, IntPtr p);
        
        [DllImport("libnodave.dll", EntryPoint = "daveExecReadRequest")]
        internal static extern int DaveExecReadRequest(IntPtr dc, IntPtr p, IntPtr rl);
        
        [DllImport("libnodave.dll", EntryPoint = "daveExecWriteRequest")]
        internal static extern int DaveExecWriteRequest(IntPtr dc, IntPtr p, IntPtr rl);
        
        [DllImport("libnodave.dll", EntryPoint = "daveUseResult")]
        internal static extern int DaveUseResult(IntPtr dc, IntPtr rs, int number);
        
        [DllImport("libnodave.dll", EntryPoint = "daveReadSZL")]
        internal static extern int DaveReadSZL(IntPtr dc, int id, int index, byte[] ddd, int len);
        
        [DllImport("libnodave.dll", EntryPoint = "daveStart")]
        internal static extern int DaveStart(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveStop")]
        internal static extern int DaveStop(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveForce200")]
        internal static extern int DaveForce200(IntPtr dc, int area, int start, int val);
        
        [DllImport("libnodave.dll", EntryPoint = "daveForceDisconnectIBH")]
        internal static extern int DaveForceDisconnectIBH(IntPtr dc, int src, int dest, int MPI);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetResponse")]
        internal static extern int DaveGetResponse(IntPtr dc);
        
        [DllImport("libnodave.dll", EntryPoint = "daveSendMessage")]
        internal static extern int DaveSendMessage(IntPtr dc, IntPtr p);
        
        [DllImport("libnodave.dll", EntryPoint = "daveGetProgramBlock")]
        internal static extern int DaveGetProgramBlock(IntPtr dc, int blockType, int number, byte[] buffer, ref int length);
        
        [DllImport("libnodave.dll", EntryPoint = "daveListBlocksOfType")]
        internal static extern int DaveListBlocksOfType(IntPtr dc, int blockType, byte[] buffer);

        #endregion

        #region DaveInterface
        [DllImport("libnodave.dll", EntryPoint = "daveNewInterface")]
        internal static extern IntPtr DaveNewInterface(
                [MarshalAs(UnmanagedType.Struct)] FD fd,
                [MarshalAs(UnmanagedType.LPWStr)] string name,
                int localMPI,
                int useProto,
                int speed
            );

        [DllImport("libnodave.dll", EntryPoint = "daveInitAdapter")]
        internal static extern int DaveInitAdapter(IntPtr di);

        [DllImport("libnodave.dll", EntryPoint = "daveListReachablePartners")]
        internal static extern int DaveListReachablePartners(IntPtr di, byte[] buffer);

        [DllImport("libnodave.dll", EntryPoint = "daveSetTimeout")]
        internal static extern void DaveSetTimeout(IntPtr di, int time);
        [DllImport("libnodave.dll", EntryPoint = "daveGetTimeout")]
        internal static extern int DaveGetTimeout(IntPtr di);

        [DllImport("libnodave.dll", EntryPoint = "daveDisconnectAdapter")]
        internal static extern IntPtr DaveDisconnectAdapter(IntPtr di);

        [DllImport("libnodave.dll", CharSet = CharSet.Unicode)]
        internal static extern string DaveGetName(IntPtr di);
        #endregion

        #region LibNodeave
        [DllImport("libnodave.dll", EntryPoint = "daveStrError")]
        internal static extern IntPtr DaveStrError(int res);

        [DllImport("libnodave.dll", EntryPoint = "daveSetDebug")]
        internal static extern void DaveSetDebug(int newDebugLevel);

        [DllImport("libnodave.dll", EntryPoint = "daveSetDebug")]
        internal static extern int DaveGetDebug();

        [DllImport("libnodave.dll", EntryPoint = "setPort")]
        internal static extern int SetPort([MarshalAs(UnmanagedType.LPWStr)]string portName, [MarshalAs(UnmanagedType.LPWStr)]string baud, int parity);

        [DllImport("libnodave.dll", EntryPoint = "closePort")]
        internal static extern int ClosePort(int port);

        [DllImport("libnodave.dll", EntryPoint = "openS7Online")]
        internal static extern int OpenS7Online([MarshalAs(UnmanagedType.LPWStr)]string portName);

        [DllImport("libnodave.dll", EntryPoint = "closeS7Online")]
        internal static extern int CloseS7Online(int port);

        [DllImport("libnodave.dll", EntryPoint = "daveAreaName")]
        internal static extern int DaveAreaName(int area);

        [DllImport("libnodave.dll", EntryPoint = "daveBlockName")]
        internal static extern int DaveBlockName(int blockType);

        [DllImport("libnodave.dll", EntryPoint = "daveStringCopy")]
        internal static extern void DaveStringCopy(int i, byte[] c);
        #endregion

        #region PDU
        [DllImport("libnodave.dll", EntryPoint = "daveNewPDU")]
        internal static extern IntPtr DaveNewPDU();

        [DllImport("libnodave.dll", EntryPoint = "daveAddVarToReadRequest")]
        internal static extern void DaveAddVarToReadRequest(IntPtr p, int area, int DBnum, int start, int bytes);

        [DllImport("libnodave.dll", EntryPoint = "daveAddBitVarToReadRequest")]
        internal static extern void DaveAddBitVarToReadRequest(IntPtr p, int area, int DBnum, int start, int bytes);

        [DllImport("libnodave.dll", EntryPoint = "daveAddVarToWriteRequest")]
        internal static extern void DaveAddVarToWriteRequest(IntPtr p, int area, int DBnum, int start, int bytes, byte[] buffer);

        [DllImport("libnodave.dll", EntryPoint = "daveAddBitVarToWriteRequest")]
        internal static extern void DaveAddBitVarToWriteRequest(IntPtr p, int area, int DBnum, int start, int bytes, byte[] buffer);
        #endregion

        #region PseudoPointer
        [DllImport("libnodave.dll", EntryPoint = "daveFree")]
        internal static extern int DaveFree(IntPtr p);
        #endregion

        #region ResultSet
        [DllImport("libnodave.dll", EntryPoint = "DaveNewResultSet")]
        internal static extern IntPtr DaveNewResultSet();

        [DllImport("libnodave.dll", EntryPoint = "DaveFreeResults")]
        internal static extern void DaveFreeResults(IntPtr rs);

        [DllImport("libnodave.dll", EntryPoint = "DaveGetErrorOfResult")]
        internal static extern int DaveGetErrorOfResult(IntPtr rs, int number);
        #endregion
    }
}
