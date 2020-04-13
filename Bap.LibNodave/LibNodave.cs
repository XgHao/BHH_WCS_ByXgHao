using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bap.LibNodave
{
    public class LibNodave
    {
        public static int daveMPIReachable = 0x30;
        public static int daveMPIunused = 0x10;
        public static int davePartnerListSize = 126;
        public static readonly int daveMaxRawLen = 2048;

        public static string DaveStrError(int res)
        {
            //分配托管内存，并将制定的数目的字符复制从非托管内存中存储到其中的字符串
            return Marshal.PtrToStringAuto(API.DaveStrError(res));
        }

        public static PLCEnum.DebugLevel Debug
        {
            get { return (PLCEnum.DebugLevel)API.DaveGetDebug(); }
            set { API.DaveSetDebug((int)value); }
        }

        public int Setport(string portName,string baud,int parity)
        {
            return API.SetPort(portName, baud, parity);
        }

        public static int Closeport(int port)
        {
            return API.ClosePort(port);
        }

        public static int OpenS7Online(string portName)
        {
            return API.OpenS7Online(portName);
        }

        public static int CloseS7Online(int port)
        {
            return API.CloseS7Online(port);
        }

        public static string BlockName(PLCEnum.S7BlockType blockType)
        {
            byte[] s = new byte[255];
            int i = API.DaveBlockName((int)blockType);
            API.DaveStringCopy(i, s);
            string st = string.Empty;
            for (i = 0; s[i] != 0; i++)
            {
                st += (char)s[i];
            }
            return st;
        }


        public static string AreaName(PLCEnum.S7BlockType blockType)
        {
            byte[] s = new byte[255];
            int i = API.DaveAreaName((int)blockType);
            API.DaveStringCopy(i, s);
            string st = string.Empty;
            for (i = 0; s[i]!=0; i++)
            {
                st += (char)s[i];
            }
            return st;
        }
    }
}
