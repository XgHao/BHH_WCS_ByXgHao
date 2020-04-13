using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bap.LibNodave
{
    /// <summary>
    /// file descriptor，在Unix/Linux系统下，一个socket句柄，可以看做一个文件，在socket上收发数据，相当于对一个文件进行读写
    /// 所以一个socket句柄，通常也用表示文件句柄的fd来表示
    /// </summary>
    public struct FD
    {
        public int rfd;
        public int wfd;

        public override bool Equals(object obj)
        {
            return ((FD)obj).rfd == rfd && ((FD)obj).wfd == wfd;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(FD left, FD right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FD left, FD right)
        {
            return !(left == right);
        }
    }
}
