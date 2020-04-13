using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.Profinet.Siemens;

namespace COMRW
{
    public class HsControlServer
    {
        SiemensS7Net sPLC;
        public HsControlServer(string plcip)
        {
            sPLC = new SiemensS7Net(SiemensPLCS.S1500, plcip) { ConnectTimeOut = 5000 };
        }

        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <returns></returns>
        public bool HsServerConnect()
        {
            OperateResult connect = sPLC.ConnectServer();
            return connect.IsSuccess;
        }

        /// <summary>
        /// 关闭PLC
        /// </summary>
        /// <returns></returns>
        public bool HsServerCloseConnect()
        {
            OperateResult disconnect = sPLC.ConnectClose();
            return disconnect.IsSuccess;
        }

        /// <summary>
        /// 读取PLC
        /// </summary>
        /// <param name="db"></param>
        /// <param name="length"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool HsRead(string db, ushort length, ref byte[] msg)
        {
            OperateResult<byte[]> result = sPLC.Read(db, length);
            if (result.IsSuccess)
            {
                msg = result.Content;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 写入PLC
        /// </summary>
        /// <param name="db"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool HsWrite(string db,byte[] msg)
        {
            OperateResult result = sPLC.Write(db, msg);
            return result.IsSuccess;
        }
    }
}
