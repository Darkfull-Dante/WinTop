using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenHardwareMonitor.Hardware;
using WinTop.Graphics;

namespace WinTop.Components
{
    class TemperatureSensor
    {
        /// <summary>
        /// 
        /// </summary>
        public string Hardware { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public ISensor Sensor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float CurrentValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float CurrentMax { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float CurrentMin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hardware"></param>
        public TemperatureSensor(IHardware hardware)
        {
            Hardware = hardware.HardwareType.ToString();

            switch (hardware.HardwareType)
            {
                case HardwareType.CPU:

                    foreach(ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name.ToUpper().Contains("PACKAGE"))
                        {
                            Sensor = sensor;
                            break;
                        }
                    }

                    break;
                
                default:

                    foreach(ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            Sensor = sensor;
                            break;
                        }
                    }

                    break;
            }
        }

        public void Update()
        {

            Sensor.Hardware.Update();
            
            CurrentValue = (float)Sensor.Value;
            CurrentMin = (float)Sensor.Min;
            CurrentMax = (float)Sensor.Max;
        }

        private string ShortenedString(string s, int n)
        {
            return s.Length < n ? s.PadRight(n) : s.Substring(0, n);
        }


        public override string ToString()
        {
            return string.Format("{0} {1}°C {2}°C {3}°C", ShortenedString(Hardware, 9), CurrentValue.ToString("N0").PadLeft(3), CurrentMin.ToString("N0").PadLeft(3), CurrentMax.ToString("N0").PadLeft(3));
        }

        public static void Print(List<TemperatureSensor> temperatureSensors, Frame frame)
        {
            //print the title line
            Program.screenBuffer.SetCursorPosition(frame.PosX + 1, frame.PosY + 1);
            Program.screenBuffer.Write("Sensor    Value Min   Max");

            //check the max possible number of sensor to print
            int maxSensor = Math.Min(temperatureSensors.Count, frame.Height - 2);

            for (int i = 0; i < maxSensor; i++)
            {
                Program.screenBuffer.SetCursorPosition(frame.PosX + 1, frame.PosY + 2 + i);
                Program.screenBuffer.Write(temperatureSensors[i].ToString());
            }
        }
    }
}
