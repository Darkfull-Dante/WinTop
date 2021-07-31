using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WinTop
{
    class Program
    {
        static void Main()
        {

            //initiate elements
            List<Frame> appFrames = Create.Frames();
            CPU[] cpuCores = Create.CPUCores();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("{0:f}%", cpuCores[0].UpdateValue());
                Thread.Sleep(1000);
            }

            Console.WriteLine();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("{0:f}%", cpuCores[0].History[i]);
            }

            Console.ReadKey();
        }

        
    }
}
