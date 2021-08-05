using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WinTop.Graphics;

namespace WinTop.Components
{
    class CPU
    {

        /// <summary>
        /// Maximum history for the object data queue
        /// </summary>
        private const int MAX_HISTORY = 1000;

        /// <summary>
        /// Array of the different console color used for the cpu line chart
        /// </summary>
        private readonly static ConsoleColor[] COLOR_VALUES = { ConsoleColor.Blue, 
                                                                ConsoleColor.DarkYellow, 
                                                                ConsoleColor.Green,
                                                                ConsoleColor.Red, 
                                                                ConsoleColor.Magenta };

        /// <summary>
        /// the performance counter element that is used to calculate the current CPU core usage
        /// </summary>
        public PerformanceCounter Counter { get; private set; }
        
        /// <summary>
        /// the numeric ID of the CPU core
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// the queue holding the cpu core usage values in order
        /// </summary>
        public Queue<float> History { get; set; }

        /// <summary>
        /// the console color value to draw the line chart and write the cpu core information
        /// </summary>
        public ConsoleColor Color { get; private set; }

        /// <summary>
        /// the line chart element that the cpu can use to draw its usage values
        /// </summary>
        public Chart LineChart { get; set; }

        /// <summary>
        /// the index (inside a collection) of the frame in which the cpu information are drawn
        /// </summary>
        public int FrameIndex { get; set; }

        /// <summary>
        /// object constructor of the the CPU class
        /// </summary>
        /// <param name="counter">the performance counter element that is used to calculate the current CPU core usage</param>
        /// <param name="lineChart">the line chart element that the cpu can use to draw its usage values</param>
        /// <param name="frameIndex">the index (inside a collection) of the frame in which the cpu information are drawn</param>
        public CPU(PerformanceCounter counter, Chart lineChart, int frameIndex)
        {
            Counter = counter;
            ID = int.Parse(counter.InstanceName);
            History = new Queue<float>();
            Color = CPUColor(ID);
            LineChart = lineChart;
            FrameIndex = frameIndex;
        }

        /// <summary>
        /// fetches the most current value of the cpu core counter and queues it in the history queue. An item is dequeued if the maximum number of item is reached
        /// </summary>
        /// <returns>the most current value of the cpu core usage</returns>
        public float Update()
        {
            float currentValue = Counter.NextValue();
            History.Enqueue(currentValue);

            //dequeue if too much values
            while (History.Count > MAX_HISTORY)
            {
                History.Dequeue();
            }

            //Update chart dataset
            LineChart.UpdateDataSet(History.ToList());

            return currentValue;
        }

        /// <summary>
        /// static class that determines the color of the cpu based on a loop around the array
        /// </summary>
        /// <param name="counterNumber">the cpu core number (zero-based)</param>
        /// <returns>the console color value of the cpu</returns>
        public static ConsoleColor CPUColor(int counterNumber)
        {
            return COLOR_VALUES[counterNumber % COLOR_VALUES.Length];
        }

        /// <summary>
        /// returns a string representing the cpu core ID followed by the most recent value in the history queue
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            List<float> tempList = History.ToList();

            return string.Format("CPU{0}:{1}%", ID, tempList[tempList.Count - 1].ToString("f").PadLeft(6));
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
                if (h + stringLength <= frame.PosX + frame.Width - 2)
                {
                    //set cursor position and console color
                    Program.screenBuffer.SetCursorPosition(h, v);

                    //print the value
                    try
                    {
                        Program.screenBuffer.Write(string.Format(" {0}", cpuValue), cpuCores[i].Color);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw;
                    }
                }
                else
                {
                    break;
                }


            }

            int[] result = { 1, numberOfRows };

            return result;
        }
    }
}
