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
        private const int MAX_HISOTRY = 1000;
        public const ConsoleColor CHART_COLOR = ConsoleColor.DarkMagenta;
        
        private ComputerInfo Computer { get; set; }
        public Queue<float> History { get; set; }
        public Chart AreaChart { get; set; }
        public int FrameIndex { get; set; }
        public ulong TotalMemory { get; set; }
        public ulong AvailableMemory { get; set; }

        public Memory(ComputerInfo computer, Chart areaChart, int frameIndex)
        {
            Computer = computer;
            History = new Queue<float>();
            AreaChart = areaChart;
            FrameIndex = frameIndex;
            TotalMemory = Computer.TotalPhysicalMemory;
            AvailableMemory = Computer.AvailablePhysicalMemory;
        }

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
            Program.screenBuffer.Write(string.Format("Available: {0}", Disk.GetReadableSize(AvailableMemory).PadLeft(9)));

            return new int[] { 21, 3 };
        }

    }
}
