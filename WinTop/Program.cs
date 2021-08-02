using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTop
{
    class Program
    {
        static void Main()
        {

            //initiate elements
            List<Frame> appFrames = Create.Frames();
            CPU[] cpuCores = Create.CPUCores(appFrames);

            Console.CursorVisible = false;

            while (true)
            {
                Frame.UpdateFrame(appFrames);


                for (int i = (cpuCores.Length - 1); i >= 0; i--)
                {
                    cpuCores[i].UpdateValue();
                    cpuCores[i].LineChart.UpdatePosition(appFrames[cpuCores[i].FrameIndex]);
                    cpuCores[i].LineChart.PrintChart();
                }
                

                Thread.Sleep(500);
            }
        }
    }
} 
