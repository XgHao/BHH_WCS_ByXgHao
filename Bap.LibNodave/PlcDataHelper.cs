using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bap.LibNodave
{
    public static class PlcDataHelper
    {
        public static short GetS16From(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[2];
                b1[1] = b[pos + 0];
                b1[0] = b[pos + 1];
                return BitConverter.ToInt16(b1, 0);
            }
            return BitConverter.ToInt16(b, pos);
        }

        public static ushort GetU16From(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[2];
                b1[1] = b[pos + 0];
                b1[0] = b[pos + 1];
                return BitConverter.ToUInt16(b1, 0);
            }
            return BitConverter.ToUInt16(b, pos);
        }

        public static int GetS32From(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[4];
                b1[3] = b[pos];
                b1[2] = b[pos + 1];
                b1[1] = b[pos + 2];
                b1[0] = b[pos + 3];
                return BitConverter.ToInt32(b1, 0);
            }
            return BitConverter.ToInt32(b, pos);
        }

        public static uint GetU32From(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[4];
                b1[3] = b[pos];
                b1[2] = b[pos + 1];
                b1[1] = b[pos + 2];
                b1[0] = b[pos + 3];
                return BitConverter.ToUInt32(b1, 0);
            }
            return BitConverter.ToUInt32(b, pos);
        }

        public static float GetFloatFrom(byte[] b, int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] b1 = new byte[4];
                b1[3] = b[pos];
                b1[2] = b[pos + 1];
                b1[1] = b[pos + 2];
                b1[0] = b[pos + 3];
                return BitConverter.ToSingle(b1, 0);
            }
            return BitConverter.ToSingle(b, pos);
        }

        public static short[] GetS16Array(byte[] b)
        {
            List<short> sl = new List<short>();
            for (int i = 0; i < b.Length - 2; i += 2) 
            {
                sl.Add(GetS16From(b, i));
            }
            return sl.ToArray();
        }

        public static ushort[] GetU16Array(byte[] b)
        {
            List<ushort> sl = new List<ushort>();
            for (int i = 0; i < b.Length - 2; i += 2) 
            {
                sl.Add(GetU16From(b, i));
            }
            return sl.ToArray();
        }

        public static int[] GetS32Array(byte[] b)
        {
            List<int> sl = new List<int>();
            for (int i = 0; i < b.Length - 4; i += 4) 
            {
                sl.Add(GetS32From(b, i));
            }
            return sl.ToArray();
        }

        public static uint[] GetU32Array(byte[] b)
        {
            List<uint> sl = new List<uint>();
            for (int i = 0; i < b.Length - 4; i += 4) 
            {
                sl.Add(GetU32From(b, i));
            }
            return sl.ToArray();
        }

        public static byte[] SetPlcData(dynamic s)
        {
            return BitConverter.IsLittleEndian ? BitConverter.GetBytes(s).Reverse().ToArray() : BitConverter.GetBytes(s);
        }

        public static byte[] SetPlcData(dynamic[] sl)
        {
            List<byte> lst = new List<byte>();
            foreach (var item in sl)
            {
                lst.AddRange(SetPlcData(item));
            }
            return lst.ToArray();
        }


        [DllImport("libnodave.dll", EntryPoint = "toPLCfloat")]
        private static extern float ToPLCfloat(float f);
        public static float ToPlcFloat(float f)
        {
            return ToPLCfloat(f);
        }


        [DllImport("libnodave.dll", EntryPoint = "daveToPLCfloat")]
        private static extern int DaveToPLCfloat(float f);
        public static int DaveToPlcfloat(float f)
        {
            return DaveToPLCfloat(f);
        }

        
        [DllImport("libnodave.dll", EntryPoint = "daveSwapIed_32")]
        private static extern int DaveSwapIed_32(int i);
        public static int DaveswapIed_32(int i)
        {
            return DaveSwapIed_32(i);
        }


        [DllImport("libnodave.dll", EntryPoint = "daveSwapIed_16")]
        private static extern int DaveSwapIed_16(int i);
        public static int DaveswapIed_16(int i)
        {
            return DaveSwapIed_16(i);
        }
    }
}
