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
        public float[] History { get; set; }
        public ConsoleColor Color { get; set; }

        public CPU(PerformanceCounter counter)
        {
            Counter = counter;
            History = new float[MAX_HISTORY];
            Color = COLOR_VALUES[int.Parse(counter.InstanceName) % 5];
        }

        public float UpdateValue()
        {
            float currentValue;
            ShiftArray(1, currentValue = Counter.NextValue());

            return currentValue;

        }

        private void ShiftArray(int shift, float newValue)
        {
            float[] arr = new float[MAX_HISTORY];

            for (int i = 0; i < History.Length; i++)
            {
                arr[(i + shift) % arr.Length] = History[i];
            }

            arr[0] = newValue;

            History = arr;

        }
    }
}
