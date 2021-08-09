using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using WinTop.Components;
using WinTop.Graphics;

namespace WinTop
{
    
    class Program
    {

        /// <summary>
        /// Screen buffer used in the program
        /// </summary>
        public static ScreenBuffer screenBuffer = new ScreenBuffer(Console.WindowWidth, Console.WindowHeight - 1);
        
        /// <summary>
        /// List of frames used in the program
        /// </summary>
        public static List<Frame> appFrames = Create.Frames();

        /// <summary>
        /// List of cpu cores used in the program
        /// </summary>
        public static List<CPU> cpuCores = Create.CPUCores();

        /// <summary>
        /// list of the disk used in the program
        /// </summary>
        public static List<Disk> disks = Create.Disks();

        /// <summary>
        /// memory object used in the program
        /// </summary>
        public static Memory memory = Create.MemoryCounter();

        /// <summary>
        /// list of temperature sensors
        /// </summary>
        public static List<TemperatureSensor> temperatureSensors = Create.TemperatureSensors();

        /// <summary>
        /// entry point of the program
        /// </summary>
        static void Main()
        {
            
            Console.OutputEncoding = Encoding.UTF8;
            int cpuGraphCount = cpuCores.Count >= 4 ? 4 : cpuCores.Count;
            bool keepRunning = true;

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                keepRunning = false;
            };

            //loop through the program until cancel key is pressed
            while (keepRunning) { Loop(cpuGraphCount); }

            //clear the screen and formating at the end of the program
            Console.ResetColor();
            //Console.ReadKey();
            Console.Clear();

        }

        /// <summary>
        /// Main loop of the program to run until the cancel key is pressed
        /// </summary>
        /// <param name="cpuGraphCount">number of cpuCore to print</param>
        private static void Loop(int cpuGraphCount)
        {
            //TO-DO: move all the inside loop to a private method
            int visibleFrameCount = 0;

            try
            {
                //update the frames
                Frame.UpdateFrame(appFrames);

                //update the cpu graph
                visibleFrameCount += UpdateCPU(cpuGraphCount);


                //print the disks details
                visibleFrameCount += UpdateDisk();

                //update Memory history
                visibleFrameCount += UpdateMemory();

                //update the temperature frame
                visibleFrameCount += UpdateTemperature();

                //update the process frame

                //update the network frame

                //print relevant data
                if (visibleFrameCount > 0)
                {
                    screenBuffer.Print();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Window too small\nPlease resize.");
                }

                //wait before update
                Thread.Sleep(100);

            }
            catch (Exception ex)
            {
                if (ex is ArgumentOutOfRangeException || ex is IndexOutOfRangeException)
                {
                    Frame.UpdateFrame(appFrames);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                screenBuffer.Clear();
            }
        }

        /// <summary>
        /// updates the cpu information
        /// </summary>
        /// <param name="cpuGraphCount">number of cpu cores to include in the chart</param>
        /// <returns>1 if the frame is visible</returns>
        private static int UpdateCPU(int cpuGraphCount)
        {

            int asVisibleFrame = 0;
            int frameIndex = (int)Create.ProgramFrame.CpuFrame;

            if (appFrames[frameIndex].IsVisible)
            {

                asVisibleFrame = 1;

                for (int i = cpuCores.Count - 1; i >= 0; i--)
                {
                    //update the cores values
                    cpuCores[i].Update();

                    //if the core fits in the graph, add it
                    if (i < cpuGraphCount)
                    {
                        cpuCores[i].LineChart.UpdatePosition(appFrames[cpuCores[i].FrameIndex]);
                        cpuCores[i].LineChart.PrintChart();
                    }
                }

                //update the cpuValue table
                appFrames[frameIndex].ProtectedData = CPU.PrintCoreData(cpuCores, appFrames[frameIndex], 4);
            }
            else
            {
                foreach (CPU cpuCore in cpuCores)
                {
                    cpuCore.Update();
                }
            }

            return asVisibleFrame;
        }

        /// <summary>
        /// updates the disk information
        /// </summary>
        /// <returns>1 if the frame is visible</returns>
        private static int UpdateDisk()
        {
            int asVisibleFrame = 0;
            int frameIndex = (int)Create.ProgramFrame.DiskFrame;
            
            if (appFrames[frameIndex].IsVisible)
            {
                asVisibleFrame = 1;
                Disk.Print(disks, appFrames[frameIndex]);
            }

            return asVisibleFrame;
        }

        /// <summary>
        /// updates the memory information
        /// </summary>
        /// <returns>1 if the frame is visible</returns>
        private static int UpdateMemory()
        {
            int visibleFrameCount = 0;
            int frameIndex = (int)Create.ProgramFrame.MemFrame;

            memory.Update();

            //print info if frame is visible
            if (appFrames[frameIndex].IsVisible)
            {

                visibleFrameCount = 1;

                memory.AreaChart.UpdatePosition(appFrames[memory.FrameIndex]);
                memory.AreaChart.PrintChart();
                appFrames[frameIndex].ProtectedData = memory.Print();

            }

            return visibleFrameCount;
        }

        /// <summary>
        /// updates the temperature information
        /// </summary>
        /// <returns>1 if the frame is visible</returns>
        private static int UpdateTemperature()
        {
            int asVisibleFrame = 0;
            int frameIndex = (int)Create.ProgramFrame.TempFrame;

            if (appFrames[frameIndex].IsVisible)
            {
                asVisibleFrame = 1;

                foreach(TemperatureSensor temperatureSensor in temperatureSensors)
                {
                    temperatureSensor.Update();
                }

                TemperatureSensor.Print(temperatureSensors, appFrames[frameIndex]);
            }

            return asVisibleFrame;
        }
    }
} 
