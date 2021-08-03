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
        public int ID { get; set; }
        public Queue<float> History { get; set; }
        public ConsoleColor Color { get; set; }
        public Chart LineChart { get; set; }
        public int FrameIndex { get; set; }

        public CPU(PerformanceCounter counter, Chart lineChart, int frameIndex)
        {
            Counter = counter;
            ID = int.Parse(counter.InstanceName);
            History = new Queue<float>();
            Color = CPUColor(ID);
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

        public override string ToString()
        {
            List<float> tempList = History.ToList();

            return string.Format("CPU{0}:{1}%", ID, tempList[tempList.Count - 1].ToString("f").PadLeft(7));
        }

        /// <summary>
        /// method to print a table of all the CPU cores percentages
        /// </summary>
        /// <param name="cpuCores">the list object of all the cpuCores object of class CPU</param>
        /// <param name="frame">the frame in which the data must be printed</param>
        /// <param name="numberOfRows">the number of rows desired</param>
        /// <returns>an array of integer containing the {width, height} of the data inside the frame</returns>
        public static int[] PrintCoreData(List<CPU> cpuCores, Frame frame, int numberOfRows)
        {

            int currentMaxColumnLength = 0;
            int lastMaxColumnLength = 0;
            int stringLength = 0;

            for(int i = 0; i < cpuCores.Count; i++)
            {
                //get current core value
                string cpuValue = cpuCores[i].ToString();
                stringLength = cpuValue.Length;

                //reset maxColumnLength if new column
                if(i % numberOfRows == 0)
                {
                    lastMaxColumnLength = currentMaxColumnLength;
                }

                //update max value
                if(stringLength + lastMaxColumnLength > currentMaxColumnLength) { currentMaxColumnLength = stringLength + lastMaxColumnLength; }
                
                //get position value
                int v = frame.PosY + 1 + (i % numberOfRows);
                int h = frame.PosX + 1 + lastMaxColumnLength + 2 * (i / numberOfRows);

                //check if position is possible and data can fit
                if (h + stringLength <= frame.PosX + frame.Width - 1)
                {
                    //set cursor position and console color
                    Console.SetCursorPosition(h, v);
                    Console.ForegroundColor = cpuCores[i].Color;

                    //print the value
                    Console.Write(" {0}", cpuValue);

                }
                else
                {
                    break;
                }


            }

            Console.ResetColor();

            int[] result = { stringLength + lastMaxColumnLength + 1, numberOfRows };

            return result;
        }
    }
}
