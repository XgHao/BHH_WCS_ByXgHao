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
