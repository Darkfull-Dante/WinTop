using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WinTop.Graphics;

namespace WinTop.Components
{
    class NetworkCounter
    {

        private int MAX_HISTORY = 1000;

        public PerformanceCounter Counter { get; set; }
        public string Name { get; set; }
        public float CurrentValue { get; set; }
        public Queue<float> History { get; set; }
        public ConsoleColor Color { get; set; }
        public Chart AreaChart { get; set; }
        public int FrameIndex { get; set; }

        public NetworkCounter(string name, PerformanceCounter counter, Queue<float> history,ConsoleColor color, Chart areaChart, int frameIndex)
        {
            Name = name;
            Counter = counter;
            History = history;
            Color = color;
            AreaChart = areaChart;
            FrameIndex = frameIndex;

            Update();
        }

        public NetworkCounter(string name) : this(name, null, null, Console.ForegroundColor, null, 0) { }

        public NetworkCounter(PerformanceCounter counter, ConsoleColor color, int frameIndex) : this(counter.CounterName, counter, new Queue<float>(), color, new Chart(Chart.LineType.AreaChart, Program.appFrames[frameIndex], color, frameIndex), frameIndex) { }

        public float Update()
        {
            CurrentValue = Counter.NextValue();
            History.Enqueue(CurrentValue);

            //dequeue if too much values
            while (History.Count > MAX_HISTORY)
            {
                History.Dequeue();
            }

            //Update chart dataset
            AreaChart.UpdateDataSet(History.ToList());

            return CurrentValue;
        }
        
    }
}
