using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitList.Core.Extensions
{
    public static class LongExtensions
    {
        public static double BytesToMegabytes(this long Bytes)
        {
            return Math.Round(Bytes / 1024.0 / 1024.0, 2);
        }
    }
}
