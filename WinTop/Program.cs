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
        /// the timer object managing time keeping
        /// </summary>
        private static Timer timer = null;

        /// <summary>
        /// boolean indicating if the main loop is busy or not to prevent race condition
        /// </summary>
        private static bool isBusy = false;

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
        /// list of process counters
        /// </summary>
        public static List<ProcessCounter> processCounters = Create.ProcessCounters();

        /// <summary>
        /// entry point of the program
        /// </summary>
        static void Main(string[] args)
        {
            
            Console.OutputEncoding = Encoding.UTF8;
            int cpuGraphCount = cpuCores.Count >= 4 ? 4 : cpuCores.Count;
            bool keepRunning = true;

            timer = new Timer(Loop, cpuGraphCount, 0, 100);

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                keepRunning = false;
            };

            while (keepRunning) { }

            timer.Dispose();

            //clear the screen and formating at the end of the program
            Console.ResetColor();
            Console.Clear();
            

            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) {}

        }

        /// <summary>
        /// Main loop of the program to run until the cancel key is pressed
        /// </summary>
        /// <param name="o">Object containing the number of cpuCore to print</param>
        private static void Loop(object o)
        {
            int cpuGraphCount = (int)o;
            int visibleFrameCount = 0;

            if (isBusy)
            {
                return;
            }

            try
            {
                isBusy = true;

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
                visibleFrameCount += UpdateProcesses();

                //update the network frame
                visibleFrameCount += UpdateNetwork();

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
                //Thread.Sleep(100);

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
                isBusy = false;
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

        /// <summary>
        /// updates the process counter information
        /// </summary>
        /// <returns>1 if frame is visible</returns>
        private static int UpdateProcesses()
        {

            const int FRAME_INDEX = (int)Create.ProgramFrame.PrcFrame;
            int asVisibleFrame = 0;
            

            if (appFrames[FRAME_INDEX].IsVisible)
            {

                asVisibleFrame = 1;

                List<Process> processes = Process.GetProcesses().ToList();

                foreach (Process process in processes)
                {
                    if (!string.IsNullOrEmpty(process.MainWindowTitle) && !processCounters.Contains(new ProcessCounter(process, true)) && process.ProcessName != Process.GetCurrentProcess().ProcessName)
                    {
                        processCounters.Add((ProcessCounter)process);
                    }
                }

                restart:
                foreach (ProcessCounter processCounter in processCounters)
                {
                    try
                    {
                        processCounter.Update();
                    }
                    catch (InvalidOperationException)
                    {
                        processCounters.Remove(processCounter);
                        goto restart;
                    }
                }

                ProcessCounter.Print(processCounters, appFrames[FRAME_INDEX]);

            }

            return asVisibleFrame;
        }

        //not yet implemented
        private static int UpdateNetwork()
        {
            return 0;
        }
    }
} 
