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
            CurrentValue = (float)Sensor.Value;
            CurrentMin = (float)Sensor.Min;
            CurrentMax = (float)Sensor.Max;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1:f}\t{2:f}\t{3:f}", Hardware, CurrentValue, CurrentMin, CurrentMax);
        }

        public static void Print(List<TemperatureSensor> temperatureSensors, Frame frame)
        {
            
        }
    }
}
