using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTop.Graphics
{
    class Chart
    {
        
        /// <summary>
        /// enumeration of the types of charts possible in the class
        /// </summary>
        public enum LineType { LineChart, AreaChart};

        /// <summary>
        /// value representing an area chart type
        /// </summary>
        public const int AREA_CHART = 1;

        /// <summary>
        /// character to be used for a line chart going up
        /// </summary>
        private const char LINE_UP = '┗';

        /// <summary>
        /// character to be used for a line chart going down
        /// </summary>
        private const char LINE_DOWN = '┏';

        /// <summary>
        /// character to be used for a line chart keepinbg its current value
        /// </summary>
        private const char LINE_HORIZONTAL = '━';

        /// <summary>
        /// character to be used for a line chart for vertical scaling
        /// </summary>
        private const char LINE_VERTICAL = '┃';

        /// <summary>
        /// character to be used for a line chart for a vertical end coming from up
        /// </summary>
        private const char LINE_FROM_UP = '┛';

        /// <summary>
        /// character to be used for a line chart for a vertical end coming from down
        /// </summary>
        private const char LINE_FROM_DOWN = '┓';

        /// <summary>
        /// character to be used for a full block in an area chart
        /// </summary>
        private const char AREA_FULLBLOCK = '█';

        /// <summary>
        /// character to be used for an half block in an area chart
        /// </summary>
        private const char AREA_HALFBLOCK = '▄';

        /// <summary>
        /// integer representing the chart type
        /// </summary>
        public LineType Type { get; set; }

        /// <summary>
        /// Collection of the data set the chart uses to prints itself
        /// </summary>
        public List<float> DataSet { get; set; }

        /// <summary>
        /// the maximal value the chart can take. If zero, the maximum will be determined by the biggest value in the dataset at the moment of printing
        /// </summary>
        public float MaxValue { get; set; }

        /// <summary>
        /// the starting index on the x axis
        /// </summary>
        public int StartX { get; set; }

        /// <summary>
        /// the starting index on the y axis
        /// </summary>
        public int StartY { get; set; }

        /// <summary>
        /// the width of the chart
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// the height of the chart
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// the color of the chart
        /// </summary>
        public ConsoleColor ChartColor { get; set; }

        /// <summary>
        /// the index (inside a collection) of the frame in which the chart is printed
        /// </summary>
        public int FrameIndex { get; set; }

        /// <summary>
        /// the area in the frame that shouldn't receive chart characters
        /// </summary>
        public int[] ProtectedData { get; set; }

        /// <summary>
        /// object constructor of the Chart class
        /// </summary>
        /// <param name="type">Type of chart to print (Line or Area)</param>
        /// <param name="datatSet">set of data to print</param>
        /// <param name="maxValue">the maximum value that can exist in the dataset. If zero, will take the maxValue of the current dataset</param>
        /// <param name="startX">Horizontal start position (top left)</param>
        /// <param name="startY">Vertical start posotion (top left)</param>
        /// <param name="width">width of the chart</param>
        /// <param name="height">height of the chart</param>
        /// <param name="chartColor">Color of the chart</param>
        /// <param name="frameIndex">index of the frame the chart is printed in</param>
        public Chart(LineType type, List<float> datatSet, float maxValue, int startX, int startY, int width, int height, ConsoleColor chartColor, int frameIndex)
        {
            Type = type;
            DataSet = datatSet;
            StartX = startX;
            MaxValue = maxValue;
            StartY = startY;
            Width = width;
            Height = height;
            ChartColor = chartColor;
            FrameIndex = frameIndex;
            ProtectedData = new int[] { 0, 0 };
        }

        /// <summary>
        /// object constructor of the Chart class
        /// </summary>
        /// <param name="type">Type of chart to print (Line or Area)</param>
        /// <param name="datatSet">set of data to print</param>
        /// <param name="maxValue">the maximum value that can exist in the dataset (can be dynamically modified for moving max</param>
        /// <param name="frame">Frame object in which the chart resides</param>
        /// <param name="chartColor">Color of the chart</param>
        /// <param name="frameIndex">index of the frame the chart is printed in</param>
        public Chart(LineType type, List<float> datatSet, float maxValue, Frame frame, ConsoleColor chartColor, int frameIndex) : this(type, datatSet, maxValue,frame.PosX + 1, frame.PosY + 1, frame.Width - 2, frame.Height - 2, chartColor, frameIndex) { }

        /// <summary>
        /// object constructor of the Chart class
        /// </summary>
        /// <param name="type">Type of chart to print (Line or Area)</param>
        /// <param name="frame">Frame object in which the chart resides</param>
        /// <param name="chartColor">Color of the chart</param>
        /// <param name="frameIndex">index of the frame the chart is printed in</param>
        public Chart(LineType type, Frame frame, ConsoleColor chartColor, int frameIndex) : this(type, new List<float>(), 100,frame.PosX + 1, frame.PosY + 1, frame.Width - 2, frame.Height - 2, chartColor, frameIndex) { }

        /// <summary>
        /// object constructor of the Chart class
        /// </summary>
        /// <param name="frame">Frame object in which the chart resides</param>
        /// <param name="chartColor">Color of the chart</param>
        /// <param name="frameIndex">index of the frame the chart is printed in</param>
        public Chart(Frame frame, ConsoleColor chartColor, int frameIndex) : this(LineType.LineChart, new List<float>(), 100,frame.PosX + 1, frame.PosY + 1, frame.Width - 2, frame.Height - 2, chartColor, frameIndex) { }

        /// <summary>
        /// Method that updates the starts and size property of chart based on a frame object
        /// </summary>
        /// <param name="frame">the frame in which the chart resides</param>
        public void UpdatePosition(Frame frame)
        {
            StartX = frame.PosX + 1;
            StartY = frame.PosY + 1;
            Width = frame.Width - 2;
            Height = frame.Height - 2;
            ProtectedData = new int[] { frame.ProtectedData[0] + 1, frame.ProtectedData[1] + 1 };
        }

        /// <summary>
        /// prints the charts in the screen buffer
        /// </summary>
        public void PrintChart()
        {
            
            switch (Type)
            {
                case LineType.LineChart:
                    PrintLineChart();
                    break;
                case LineType.AreaChart:
                    PrintAreaChart();
                    break;
            }
        }

        /// <summary>
        /// method to print the area chart
        /// </summary>
        private void PrintAreaChart()
        {
            int hStart = StartX + Width - DataSet.Count;

            for (int i = DataSet.Count - 1; i >= 0; i--)
            {
                float max;
                int value;

                try
                {
                    max = MaxValue != 0 ? MaxValue : DataSet.Max();
                    value = VerticalValue(DataSet[i], max, Height);

                    //set the cursor posotion
                    Program.screenBuffer.SetCursorPosition(hStart + i, StartY + Height - 1);
                    PrintAreaLine(DataSet[i], value);
                }
                catch (IndexOutOfRangeException)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// prints a full column of an area chart
        /// </summary>
        /// <param name="realValue">the real value of th class object</param>
        /// <param name="indexValue">the height value of the current column in terms of the maximum height of the chart</param>
        private void PrintAreaLine(float realValue, int indexValue)
        {
            int vEnd = Program.screenBuffer.CursorTop - indexValue;
            int i;
            
            for (i = Program.screenBuffer.CursorTop; i > vEnd; i--)
            {
                Program.screenBuffer.CursorTop = i;
                if(!IsInProtectedZone()) { Program.screenBuffer.Write(AREA_FULLBLOCK, ChartColor); }
                Program.screenBuffer.CursorLeft--;
            }

            if ((int)realValue / 10 >= 5)
            {
                Program.screenBuffer.CursorTop = i;
                if (!IsInProtectedZone()) { Program.screenBuffer.Write(AREA_HALFBLOCK, ChartColor); }
                Program.screenBuffer.CursorLeft--;
            } 
        }

        /// <summary>
        /// method to print a line chart
        /// </summary>
        private void PrintLineChart()
        {

            int hStart = StartX + Width - DataSet.Count;

            for (int i = DataSet.Count - 1; i >= 0; i--)
            {

                float max;
                int presentValue;
                int olderValue;

                try
                {
                    max = MaxValue != 0 ? MaxValue : DataSet.Max();

                    presentValue = VerticalValue(DataSet[i], max, Height);
                    olderValue = (i != 0) ? VerticalValue(DataSet[i - 1], max, Height) : presentValue;

                    //set cursor posotion
                    Program.screenBuffer.SetCursorPosition(hStart + i, StartY + Height - presentValue - 1);
                    //break;
                }
                catch (IndexOutOfRangeException)
                {
                    throw;
                }

                //print the char
                try
                {
                    if (!IsInProtectedZone()) { Program.screenBuffer.Write(GetLineChar(presentValue, olderValue), ChartColor); }
                }
                catch (IndexOutOfRangeException)
                {
                    throw;
                }
                
                
                //print the ascending or descending line
                PrintVeticalLines(presentValue, olderValue, hStart + i);
            }

            Console.ResetColor();
        }

        /// <summary>
        /// checks if the cursor position in the screenbuffer is inside a protected zone
        /// </summary>
        /// <returns>true if not in protected zone</returns>
        private bool IsInProtectedZone()
        {
            return Program.screenBuffer.CursorLeft <= ProtectedData[0] && Program.screenBuffer.CursorTop <= ProtectedData[1];
        }

        /// <summary>
        /// prints the vertical line between two extremity of a vertical in a line chart
        /// </summary>
        /// <param name="presentValue">the current value of the line chart</param>
        /// <param name="olderValue">the older value of the line chart (where the chart is going</param>
        /// <param name="hPos">the current horizontal position in the chart</param>
        private void PrintVeticalLines(int presentValue, int olderValue, int hPos)
        {

            int end = StartY + Height - olderValue - 1;
            int start = StartY + Height - presentValue - 1;
            char endChar = LINE_VERTICAL;
            
            //set the horizontal position
            Program.screenBuffer.CursorLeft = hPos;

            //determine direction, if both value equal, exit
            if (presentValue == olderValue)
            {
                return;
            }
            else if (presentValue > olderValue)
            {
                for (int i = start + 1; i < end; i++)
                {
                    
                    Program.screenBuffer.CursorTop = i;
                    if (!IsInProtectedZone())
                    {
                        Program.screenBuffer.Write(LINE_VERTICAL, ChartColor);
                        Program.screenBuffer.CursorLeft--;
                    }
             
                    
                }

                endChar = LINE_FROM_UP;
            }
            else if (presentValue < olderValue)
            {
                for (int i = start - 1; i > end; i--)
                {
                    Program.screenBuffer.CursorTop = i;
                    if (!IsInProtectedZone())
                    {
                        Program.screenBuffer.Write(LINE_VERTICAL, ChartColor);
                        Program.screenBuffer.CursorLeft--;
                    }
                    
                }

                endChar = LINE_FROM_DOWN;
            }

            //write the last char
            Program.screenBuffer.CursorTop = end;
            if (!IsInProtectedZone())
            {
                Program.screenBuffer.Write(endChar, ChartColor);
                Program.screenBuffer.CursorLeft--;
            }
            
        }

        /// <summary>
        /// returns the character for the going direction of the line chart
        /// </summary>
        /// <param name="presentValue">the current value of the line chart</param>
        /// <param name="olderValue">the older value (next in line_ of the line chart</param>
        /// <returns>the correctu character for continuing the line chart</returns>
        private char GetLineChar(float presentValue, float olderValue)
        {
            if (olderValue > presentValue)
            {
                return LINE_UP;
            }
            else if (olderValue == presentValue)
            {
                return LINE_HORIZONTAL;
            }
            else if (olderValue < presentValue)
            {
                return LINE_DOWN;
            }

            return LINE_DOWN;
        }

        /// <summary>
        /// updates the data set of the chart
        /// </summary>
        /// <param name="newData">the data to replace the current one</param>
        public void UpdateDataSet(List<float> newData)
        {
            //remove data out of the range of the width
            if (newData.Count > Width)
            {
                newData.RemoveRange(0, newData.Count - Width);
            }
        
            
            //reverse the dataset
            //newData.Reverse();

            DataSet = newData;
        }

        /// <summary>
        /// method to return the vertical value as a ratio of the data toward the maximum
        /// </summary>
        /// <param name="data">the current value</param>
        /// <param name="max">the maximum value</param>
        /// <param name="height">the maximum height in the chart</param>
        /// <returns>the ratio equivalent of x/height = data/max</returns>
        private static int VerticalValue(float data, float max, int height)
        {
            float percent = data / max;
            int result = (int)(percent * height);

            return result < height ? result : height - 1;
        }
    }
}
