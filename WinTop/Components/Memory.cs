using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Devices;
using WinTop.Graphics;

namespace WinTop.Components
{
    class Memory
    {

        /// <summary>
        /// array containing all relevent memory size for the disk
        /// </summary>
        private readonly static string[] MEMORY_SIZE = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <summary>
        /// Maximum history for the object data queue
        /// </summary>
        private const int MAX_HISOTRY = 1000;

        /// <summary>
        /// the console color to use for the memory area chart
        /// </summary>
        public const ConsoleColor CHART_COLOR = ConsoleColor.DarkMagenta;
        
        /// <summary>
        /// the object containing the memory information
        /// </summary>
        public ComputerInfo Computer { get; private set; }

        /// <summary>
        /// the queue holding the memory usage values in order
        /// </summary>
        public Queue<float> History { get; set; }

        /// <summary>
        /// the area chart element that the memory can use to draw its usage values
        /// </summary>
        public Chart AreaChart { get; set; }

        /// <summary>
        /// the index (inside a collection) of the frame in which the memory information is drawn
        /// </summary>
        public int FrameIndex { get; set; }

        /// <summary>
        /// the total memory of the com^puter
        /// </summary>
        public ulong TotalMemory { get; set; }
        
        /// <summary>
        /// the unsigned long value of the currently available memory in bytes
        /// </summary>
        public ulong AvailableMemory { get; set; }

        /// <summary>
        /// object constructor of the Memory class
        /// </summary>
        /// <param name="computer">the computer information object containing the memory information</param>
        /// <param name="areaChart">the area chart element that the memory can use to draw its usage values</param>
        /// <param name="frameIndex">the index (inside a collection) of the frame in which the memory information is drawn</param>
        public Memory(ComputerInfo computer, Chart areaChart, int frameIndex)
        {
            Computer = computer;
            History = new Queue<float>();
            AreaChart = areaChart;
            FrameIndex = frameIndex;
            TotalMemory = Computer.TotalPhysicalMemory;
            AvailableMemory = Computer.AvailablePhysicalMemory;
        }

        /// <summary>
        /// fetches the currently available memory and update the available memory property and adds it to the history queue
        /// </summary>
        /// <returns>the currently available memory</returns>
        public float Update()
        {

            AvailableMemory = Computer.AvailablePhysicalMemory;
            
            float currentValue = ((float)Computer.TotalPhysicalMemory - AvailableMemory) / Computer.TotalPhysicalMemory * 100;
            History.Enqueue(currentValue);

            //dequeue if too much values
            while (History.Count > MAX_HISOTRY)
            {
                History.Dequeue();
            }

            AreaChart.UpdateDataSet(History.ToList());

            return currentValue;
        }

        /// <summary>
        /// static methods that returns a readable memory size from a number of bytes
        /// </summary>
        /// <param name="space">memory size in bytes</param>
        /// <returns>readable version of a memory size with its memory size label added (ie. 100 MB)</returns>
        public static string GetReadableSize(float space)
        {
            const int DIVIDER = 1024;
            int i = 0;

            while (space / DIVIDER > 1 && i < MEMORY_SIZE.Length)
            {
                space /= DIVIDER;
                i++;
            }

            return space.ToString("f") + " " + MEMORY_SIZE[i].PadRight(2);
        }

        /// <summary>
        /// prints the memory percentage use and returns the area in which it resides
        /// </summary>
        /// <returns>an array of integer containing the {width, height} of the data inside the frame</returns>
        public int[] Print()
        {
            int posX = Program.appFrames[FrameIndex].PosX + 1;
            int posY = Program.appFrames[FrameIndex].PosY + 1;

            List<float> tempList = History.ToList();

            //write the percent used memory
            Program.screenBuffer.SetCursorPosition(posX, posY);
            Program.screenBuffer.Write(string.Format("   % used: {0} %", tempList[tempList.Count - 1].ToString("f").PadLeft(6)));

            //write the available memory
            Program.screenBuffer.SetCursorPosition(posX, posY + 1);
            Program.screenBuffer.Write(string.Format("Available: {0}", GetReadableSize(AvailableMemory).PadLeft(9)));

            return new int[] { 21, 3 };
        }
    }
}
