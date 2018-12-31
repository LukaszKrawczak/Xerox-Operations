using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xerox_operations.window
{
    public static class ApplicationController
    {

        public static void close()
        {            
            Process p1 = Process.GetCurrentProcess();            
            foreach (var process in Process.GetProcessesByName(p1.ProcessName))
            {
                process.Kill();
            }
        }
        
    }
}
