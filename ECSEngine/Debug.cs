using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSEngine
{
    public static class Debug
    {
        public static void Log(string str)
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}] - {str}");
        }
    }
}
