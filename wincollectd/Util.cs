using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wincollectd
{
    class Util
    {
        public static string GetProgramFilesDir(){
            if (IntPtr.Size == 8
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }
    }
}
