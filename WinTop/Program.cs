﻿using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WinTop
{
    class Program
    {

        public static ScreenBuffer screenBuffer = new ScreenBuffer(Console.WindowWidth, Console.WindowHeight - 1);
        public static List<Frame> appFrames = Create.Frames();
        public static List<CPU> cpuCores = Create.CPUCores(appFrames);
        

        static void Main()
        {

            Console.OutputEncoding = Encoding.UTF8;
            int cpuGraphCount = cpuCores.Count >= 4 ? 4 : cpuCores.Count;
            

            Frame.UpdateFrame(appFrames);

            while (true)
            {

                //update the frames
                Frame.UpdateFrame(appFrames);

                //update the cpu graph
                for (int i = cpuGraphCount - 1; i >= 0; i--)
                {
                    cpuCores[i].UpdateValue();
                    cpuCores[i].LineChart.UpdatePosition(appFrames[cpuCores[i].FrameIndex]);
                    cpuCores[i].LineChart.PrintChart();
                }

                //update the cpuValue table
                appFrames[Create.CPU_FRAME].ProtectedData = CPU.PrintCoreData(cpuCores, appFrames[Create.CPU_FRAME], 4);
                
                //wait before update
                Thread.Sleep(100);
                

                try
                {
                    screenBuffer.Print();
                }
                catch (IndexOutOfRangeException)
                {
                    Frame.UpdateFrame(appFrames);
                }
                finally
                {
                    screenBuffer.Clear();
                }
            }
        }
    }
} 
