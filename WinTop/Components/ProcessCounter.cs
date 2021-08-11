using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WinTop.Graphics;

namespace WinTop.Components
{
    class ProcessCounter : IComparable
    {

        /// <summary>
        /// array containing all relevent memory size for the disk
        /// </summary>
        private readonly static string[] MEMORY_SIZE = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        private const int MAX_NAME_LENGTH = 29;

        /// <summary>
        /// the name of the category for to performance counter
        /// </summary>
        private const string CATEGORY = "Process";

        /// <summary>
        /// name of the counter for the processor value
        /// </summary>
        private const string PROC_COUNTER = "% Processor Time";

        /// <summary>
        /// name of the counter for the memory value
        /// </summary>
        private const string MEM_COUNTER = "Private Bytes";

        /// <summary>
        /// Name of the process
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The performance counter keeping track of the cpu usage
        /// </summary>
        private PerformanceCounter ProcUsage { get; set; }

        /// <summary>
        /// The performance counter keeping track of the memory usage
        /// </summary>
        private PerformanceCounter MemUsage { get; set; }

        /// <summary>
        /// the last processor usage registrered
        /// </summary>
        public float CurrentProcUsage { get; private set; }

        /// <summary>
        /// the last memory usage registered
        /// </summary>
        public float CurrentMemUsage { get; private set; }

        /// <summary>
        /// object constructor of the process counter class
        /// </summary>
        /// <param name="process">the process object to track usage</param>
        public ProcessCounter(Process process) : this(process, false) { }

        /// <summary>
        /// object constructor of the process counter class
        /// </summary>
        /// <param name="process">the process object to track usage</param>
        /// <param name="isEmptyObject">set true if an empty object must be returned (for comparisson pruposes)</param>
        public ProcessCounter(Process process, bool isEmptyObject)
        {
            Name = process.ProcessName;

            if (!isEmptyObject)
            {
                ProcUsage = new PerformanceCounter(CATEGORY, PROC_COUNTER, process.ProcessName);
                MemUsage = new PerformanceCounter(CATEGORY, MEM_COUNTER, process.ProcessName);
                Update();
            }
            else
            {
                ProcUsage = null;
                MemUsage = null;
            }
            
        }

        /// <summary>
        /// Cast operation from process to processCounter
        /// </summary>
        /// <param name="process">the process to convert to processCounter type</param>
        public static explicit operator ProcessCounter(Process process)
        {
            return new ProcessCounter(process);
        }

        /// <summary>
        /// updates the processor and memory usage values
        /// </summary>
        public void Update()
        {
            CurrentProcUsage = ProcUsage.NextValue();
            CurrentMemUsage = MemUsage.NextValue();
        }
        
        /// <summary>
        /// returns the hash code for this processCounter
        /// </summary>
        /// <returns>Hashcode of this processCounter</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// returns a formatted string value of the process counter
        /// </summary>
        /// <returns>formatted string value of the process counter</returns>
        public override string ToString()
        {

            string trimmedName = Name.Substring(0, Math.Min(MAX_NAME_LENGTH, Name.Length));
            
            return string.Format("{0} {1}% {2}", trimmedName.PadRight(MAX_NAME_LENGTH), CurrentProcUsage.ToString("f").PadLeft(6), GetReadableSize(CurrentMemUsage).PadLeft(9));
        }

        /// <summary>
        /// returns true if the object is equal to this processCounter
        /// </summary>
        /// <param name="obj">an obj to compare to</param>
        /// <returns>true if both object are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is ProcessCounter b)
            {
                return Name.Equals(b.Name);
            }
            else
            {
                throw new ArgumentException("Object is not a Process Counter");
            }
            
        }

        /// <summary>
        /// static methods that returns a readable memory size from a number of bytes
        /// </summary>
        /// <param name="space">memory size in bytes</param>
        /// <returns>readable version of a memory size with its memory size label added (ie. 100 MB)</returns>
        public static string GetReadableSize(float space)
        {
            const int DIVIDER = 1024;
            int i = 0;

            while (space / DIVIDER > 1 && i < MEMORY_SIZE.Length)
            {
                space /= DIVIDER;
                i++;
            }

            return space.ToString("f") + " " + MEMORY_SIZE[i].PadRight(2);
        }

        /// <summary>
        /// Compares this process counter to the object obj and determines there place in the order stack
        /// </summary>
        /// <param name="obj">obj to compare to</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj is ProcessCounter b)
            {
                return Name.CompareTo(b.Name);
            }
            else
            {
                throw new ArgumentException("Object is not a Process Counter");
            }
        }

        /// <summary>
        /// Prints the list of process counters provided in the frame provided in a table fashion
        /// </summary>
        /// <param name="processCounters">list of process counter to print</param>
        /// <param name="frame">frame where to print the process</param>
        public static void Print(List<ProcessCounter> processCounters, Frame frame)
        {
            int startX = frame.PosX + 1;
            int startY = frame.PosY + 1;

            //print the title row
            Program.screenBuffer.SetCursorPosition(startX, startY);
            Program.screenBuffer.Write(string.Format("{0}  {1} {2}", "Process".PadRight(MAX_NAME_LENGTH), "CPU %".PadRight(7), "Memory".PadRight(9)));

            //print all process until end of the list or space in the frame
            int maxProcess = Math.Min(frame.Height - 3, processCounters.Count);

            for (int i = 0; i < maxProcess; i++)
            {
                Program.screenBuffer.SetCursorPosition(startX, startY + 1 + i);
                Program.screenBuffer.Write(processCounters[i].ToString());
            }
        }
    }
}
