using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitList.Core.Extensions
{
    public static class StringExtensions
    {
        public static long GetDirectorySizeBytes(this string DirectoryPath)
        {
            return new DirectoryInfo(DirectoryPath).GetFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);
        }

        public static void OpenPathInExplorer(this string DirectoryPath)
        {
            Process.Start(DirectoryPath);
        }
    }
}
