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
        /// The number of frame taht are visible during a screen buffre print
        /// </summary>
        private static int visibleFrameCount = 0;

        /// <summary>
        /// Screen buffer used in the program
        /// </summary>
        public static ScreenBuffer screenBuffer = new ScreenBuffer(Console.WindowWidth, Console.WindowHeight - 1);
        
        /// <summary>
        /// List of frames used in the program
        /// </summary>
        public static List<Frame> appFrames = new List<Frame>();

        /// <summary>
        /// List of cpu cores used in the program
        /// </summary>
        public static List<CPU> cpuCores = new List<CPU>();

        /// <summary>
        /// list of the disk used in the program
        /// </summary>
        public static List<Disk> disks = new List<Disk>();

        /// <summary>
        /// memory object used in the program
        /// </summary>
        public static Memory memory = new Memory();

        /// <summary>
        /// list of temperature sensors
        /// </summary>
        public static List<TemperatureSensor> temperatureSensors = new List<TemperatureSensor>();

        /// <summary>
        /// list of process counters
        /// </summary>
        public static List<ProcessCounter> processCounters = new List<ProcessCounter>();

        /// <summary>
        /// entry point of the program
        /// </summary>
        static void Main(string[] args)
        {
            
            //create the list of components
            appFrames = Create.Frames();
            cpuCores = Create.CPUCores();
            disks = Create.Disks();
            memory = Create.MemoryCounter();
            temperatureSensors = Create.TemperatureSensors();
            processCounters = Create.ProcessCounters();

            Console.OutputEncoding = Encoding.UTF8;
            int cpuGraphCount = cpuCores.Count >= 4 ? 4 : cpuCores.Count;
            bool keepRunning = true;

            timer = new Timer(Loop, cpuGraphCount, 0, 1000);

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
        private static async void Loop(object o)
        {
            int cpuGraphCount = (int)o;
            visibleFrameCount = 0;

            if (isBusy)
            {
                return;
            }

            try
            {
                isBusy = true;

                //update the frames
                Frame.UpdateFrame(appFrames);

                var tasks = new Task[]
                {
                    //update the cpu graph
                    UpdateCPU(cpuGraphCount),


                    //print the disks details
                    UpdateDisk(),

                    //update Memory history
                    UpdateMemory(),

                    //update the temperature frame
                    UpdateTemperature(),

                    //update the process frame
                    UpdateProcesses(),

                    //update the network frame
                    UpdateNetwork(),
                };

                await Task.WhenAll(tasks);

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
        private static Task UpdateCPU(int cpuGraphCount)
        {

            bool asVisibleFrame = false;
            int frameIndex = (int)Create.ProgramFrame.CpuFrame;

            if (appFrames[frameIndex].IsVisible)
            {

                asVisibleFrame = true;

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

            if (asVisibleFrame)
            {
                visibleFrameCount++;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// updates the disk information
        /// </summary>
        /// <returns>1 if the frame is visible</returns>
        private static Task UpdateDisk()
        {
            bool asVisibleFrame = false;
            int frameIndex = (int)Create.ProgramFrame.DiskFrame;
            
            if (appFrames[frameIndex].IsVisible)
            {
                asVisibleFrame = true;
                Disk.Print(disks, appFrames[frameIndex]);
            }

            if (asVisibleFrame)
            {
                visibleFrameCount++;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// updates the memory information
        /// </summary>
        /// <returns>1 if the frame is visible</returns>
        private static Task UpdateMemory()
        {
            bool asVisibleFrame = false;
            int frameIndex = (int)Create.ProgramFrame.MemFrame;

            memory.Update();

            //print info if frame is visible
            if (appFrames[frameIndex].IsVisible)
            {

                asVisibleFrame = true;

                memory.AreaChart.UpdatePosition(appFrames[memory.FrameIndex]);
                memory.AreaChart.PrintChart();
                appFrames[frameIndex].ProtectedData = memory.Print();

            }

            if (asVisibleFrame)
            {
                visibleFrameCount++;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// updates the temperature information
        /// </summary>
        /// <returns>1 if the frame is visible</returns>
        private static Task UpdateTemperature()
        {
            bool asVisibleFrame = false;
            int frameIndex = (int)Create.ProgramFrame.TempFrame;

            if (appFrames[frameIndex].IsVisible)
            {
                asVisibleFrame = true;

                foreach(TemperatureSensor temperatureSensor in temperatureSensors)
                {
                    temperatureSensor.Update();
                }

                TemperatureSensor.Print(temperatureSensors, appFrames[frameIndex]);
            }

            if (asVisibleFrame)
            {
                visibleFrameCount++;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// updates the process counter information
        /// </summary>
        /// <returns>1 if frame is visible</returns>
        private static Task UpdateProcesses()
        {

            const int FRAME_INDEX = (int)Create.ProgramFrame.PrcFrame;
            bool asVisibleFrame = false;
            

            if (appFrames[FRAME_INDEX].IsVisible)
            {

                asVisibleFrame = true;

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

                ProcessCounter.Print(new List<ProcessCounter>(processCounters.OrderByDescending(x => x.CurrentProcUsage)), appFrames[FRAME_INDEX]);

            }

            if (asVisibleFrame)
            {
                visibleFrameCount++;
            }

            return Task.CompletedTask;
        }

        //not yet implemented
        private static Task UpdateNetwork()
        {
            return Task.CompletedTask;
        }
    }
} 
