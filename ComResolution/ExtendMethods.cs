using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComResolution
{
    public static class ExtendMethods
    {
        public static int ToInt(this object dataColumn)
        {
            if (Convert.IsDBNull(dataColumn))
            {
                return 0;
            }
            return Convert.ToInt32(dataColumn);
        }
    }
}
