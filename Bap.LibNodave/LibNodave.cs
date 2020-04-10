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

        [DllImport("libnodave.dll", EntryPoint = "daveStrerror")]
        private static extern IntPtr DaveStrerror(int res);
        public static string DaveStrError(int res)
        {
            //分配托管内存，并将制定的数目的字符复制从非托管内存中存储到其中的字符串
            return Marshal.PtrToStringAuto(DaveStrerror(res));
        }


        [DllImport("libnodave.dll", EntryPoint = "daveSetDebug")]
        private static extern void DaveSetDebug(int newDebugLevel);


        [DllImport("libnodave.dll", EntryPoint = "daveSetDebug")]
        private static extern int DaveGetDebug();
        public static PLCEnum.DebugLevel Debug
        {
            get { return (PLCEnum.DebugLevel)DaveGetDebug(); }
            set { DaveSetDebug((int)value); }
        }


        [DllImport("libnodave.dll", EntryPoint = "setPort")]
        private static extern int SetPort([MarshalAs(UnmanagedType.LPWStr)]string portName, [MarshalAs(UnmanagedType.LPWStr)]string baud, int parity);
        public int Setport(string portName,string baud,int parity)
        {
            return SetPort(portName, baud, parity);
        }


        [DllImport("libnodave.dll", EntryPoint = "closePort")]
        private static extern int ClosePort(int port);
        public static int Closeport(int port)
        {
            return ClosePort(port);
        }


        [DllImport("libnodave.dll", EntryPoint = "openS7online")]
        private static extern int OpenS7online([MarshalAs(UnmanagedType.LPWStr)]string portName);
        public static int OpenS7Online(string portName)
        {
            return OpenS7online(portName);
        }


        [DllImport("libnodave.dll", EntryPoint = "closeS7online")]
        private static extern int CloseS7online(int port);
        public static int CloseS7Online(int port)
        {
            return CloseS7online(port);
        }


        [DllImport("libnodave.dll", EntryPoint = "daveAreaName")]
        private static extern int DaveAreaName(int area);


        [DllImport("libnodave.dll", EntryPoint = "daveBlockName")]
        private static extern int DaveBlockName(int blockType);


        [DllImport("libnodave.dll", EntryPoint = "daveStringCopy")]
        private static extern void DaveStringCopy(int i, byte[] c);


        public static string BlockName(PLCEnum.S7BlockType blockType)
        {
            byte[] s = new byte[255];
            int i = DaveBlockName((int)blockType);
            DaveStringCopy(i, s);
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
            int i = DaveAreaName((int)blockType);
            DaveStringCopy(i, s);
            string st = string.Empty;
            for (i = 0; s[i]!=0; i++)
            {
                st += (char)s[i];
            }
            return st;
        }
    }
}
