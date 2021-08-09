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
        /// Name of the hardware linked to the temperature 
        /// </summary>
        public string Hardware { get; private set; }
        
        /// <summary>
        /// The sensor object of the temperature
        /// </summary>
        public ISensor Sensor { get; set; }

        /// <summary>
        /// the currently treated value
        /// </summary>
        public float CurrentValue { get; set; }

        /// <summary>
        /// the maximum value of the current session
        /// </summary>
        public float CurrentMax { get; set; }

        /// <summary>
        /// the minimal value of the current session
        /// </summary>
        public float CurrentMin { get; set; }

        /// <summary>
        /// object constructor of the Temperature sensor class
        /// </summary>
        /// <param name="hardware">the hardware object from which a temperature sensor should be taken</param>
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

        /// <summary>
        /// updates the sensor object with the most up to date values
        /// </summary>
        public void Update()
        {

            Sensor.Hardware.Update();
            
            CurrentValue = (float)Sensor.Value;
            CurrentMin = (float)Sensor.Min;
            CurrentMax = (float)Sensor.Max;
        }

        /// <summary>
        /// shortens a string s to a maximum of n character. Pads it if shorter
        /// </summary>
        /// <param name="s">string to shorten</param>
        /// <param name="n">maximum number of character</param>
        /// <returns></returns>
        private string ShortenedString(string s, int n)
        {
            return s.Length < n ? s.PadRight(n) : s.Substring(0, n);
        }

        /// <summary>
        /// return a string value for a table of value of the temperature sensor class
        /// </summary>
        /// <returns>string of the hardware name, current value, min and max</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}°C {2}°C {3}°C", ShortenedString(Hardware, 9), CurrentValue.ToString("N0").PadLeft(3), CurrentMin.ToString("N0").PadLeft(3), CurrentMax.ToString("N0").PadLeft(3));
        }

        /// <summary>
        /// prints a table with title of the list of temperature sensor provided in the frame that is provided
        /// </summary>
        /// <param name="temperatureSensors"></param>
        /// <param name="frame"></param>
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
