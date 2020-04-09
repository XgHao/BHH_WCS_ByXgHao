using System;   
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bap.LibNodave
{
    public class PseudoPointer : IDisposable
    {
        private bool isDisposed;
        public IntPtr Pointer;


        [DllImport("libnodave.dll",EntryPoint = "daveFree")]
        private static extern int DaveFree(IntPtr p);

        protected static int Free(IntPtr p)
        {
            return DaveFree(p);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }
            
            if (disposing)
            {
                //释放托管资源
                Free(Pointer);
            }

            //释放本地资源
            if (Pointer != IntPtr.Zero) 
            {
                Marshal.FreeHGlobal(Pointer);
                Pointer = IntPtr.Zero;
            }

            isDisposed = true;
        }

        ~PseudoPointer()
        {
            Dispose(false);
        }
    }
}
