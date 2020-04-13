using Bap.LibNodave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMRW
{
    public class RWLOPCServerl
    {
        PlcTcpClient mPlc = null;

        public RWLOPCServerl(string ip, int port = 102)
        {
            mPlc = new PlcTcpClient(ip, port);
        }

        public bool Connect()
        {
            //连接数等于0，即连接成功
            return mPlc.ConnectPLC() == 0;
        }

        public void DisConnect()
        {
            mPlc.DisConnectPLC();
        }

        public bool Write(int DBNumber,int start,int len,byte[] msg)
        {
            if (mPlc.IsConnected)
            {
                if (mPlc.WriteManyBytes(PLCEnum.PlcMemoryArea.DB, DBNumber, start, len, msg) == 0) 
                {
                    return true;
                }
            }
            return false;
        }

        public bool Read(int Dbnumber,int start,int len,ref byte[] msg)
        {
            lock (this)
            {
                if (mPlc.IsConnected)
                {
                    if (mPlc.ReadManyBytes(PLCEnum.PlcMemoryArea.DB, Dbnumber, start, len, msg) == 0)  
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
