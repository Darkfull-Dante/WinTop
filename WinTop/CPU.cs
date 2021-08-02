using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WinTop
{
    class CPU
    {

        private const int MAX_HISTORY = 1000;
        private static ConsoleColor[] COLOR_VALUES = { ConsoleColor.Blue, 
                                                       ConsoleColor.DarkYellow, 
                                                       ConsoleColor.Green,
                                                       ConsoleColor.Red, 
                                                       ConsoleColor.Magenta };

        public PerformanceCounter Counter { get; set; }
        public Queue<float> History { get; set; }
        public ConsoleColor Color { get; set; }
        public Chart LineChart { get; set; }
        public int FrameIndex { get; set; }

        public CPU(PerformanceCounter counter, Chart lineChart, int frameIndex)
        {
            Counter = counter;
            History = new Queue<float>();
            Color = CPUColor(int.Parse(counter.InstanceName));
            LineChart = lineChart;
            FrameIndex = frameIndex;
        }

        public float UpdateValue()
        {
            float currentValue = Counter.NextValue();
            History.Enqueue(currentValue);

            //dequeue if too much values
            if (History.Count > MAX_HISTORY)
            {
                History.Dequeue();
            }

            //Update chart dataset
            LineChart.UpdateDataSet(History.ToList());

            return currentValue;
        }

        public static ConsoleColor CPUColor(int counterNumber)
        {
            return COLOR_VALUES[counterNumber % COLOR_VALUES.Length];
        }
    }
}
