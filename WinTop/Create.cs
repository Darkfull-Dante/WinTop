using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WinTop.Components;
using WinTop.Graphics;
using System.IO;
using Microsoft.VisualBasic.Devices;

namespace WinTop
{
    /// <summary>
    /// class used for the creation of different elements in the program
    /// </summary>
    class Create
    {

        /// <summary>
        /// enumaration of the different frames
        /// </summary>
        public enum ProgramFrame { CpuFrame, DiskFrame, TempFrame, MemFrame, PrcFrame, NetwFrame}

        /// <summary>
        /// creates the frames to be used by main
        /// </summary>
        /// <returns>returns a ListObjects of the frames taht were created</returns>
        public static List<Frame> Frames()
        {
            List<Frame> frames = new List<Frame>();

            frames.Add(new Frame(0, 12, "CPU Usage", 0, 0, 20, 12));
            frames.Add(new Frame(30, 7, "Disk Usage", 0, 12, 30, 7));
            frames.Add(new Frame(30, 7, "Temperatures", 0, 19, 30, 7));
            frames.Add(new Frame(0, 14, "Memory Usage", 30, 12, 21, 14));
            frames.Add(new Frame(50, 13, "Processes", 0, 26, 50, 13));
            frames.Add(new Frame(0, 13, "Network Usage", 50, 26, 19, 13));

            return frames;
        }

        /// <summary>
        /// creates the memory counter
        /// </summary>
        /// <returns>memory performance counter</returns>
        public static Memory MemoryCounter()
        {
            return new Memory(new ComputerInfo(), new Chart(Chart.LineType.AreaChart, Program.appFrames[(int)ProgramFrame.MemFrame], Memory.CHART_COLOR, (int)ProgramFrame.MemFrame), (int)ProgramFrame.MemFrame);
        }

        /// <summary>
        /// creates the cpucores list for the performance counters
        /// </summary>
        /// <returns>list of the cpu cores objects</returns>
        public static List<CPU> CPUCores()
        {
            int coreCount = Environment.ProcessorCount;


            List<CPU> cpuCores = new List<CPU>();

            for (int i = 0; i < coreCount; i++)
            {
                cpuCores.Add(new CPU(new PerformanceCounter("Processor", "% Processor Time", i.ToString()), new Chart(Program.appFrames[(int)ProgramFrame.CpuFrame], CPU.CPUColor(i), (int)ProgramFrame.CpuFrame), (int)ProgramFrame.CpuFrame));
            }

            return cpuCores;
        }

        /// <summary>
        /// creates the list of disks for uses in the disk frame
        /// </summary>
        /// <returns>list of disks</returns>
        public static List<Disk> Disks()
        {
            DriveInfo[] driveInfos = DriveInfo.GetDrives();
            List<Disk> disks = new List<Disk>();

            foreach (DriveInfo drive in driveInfos)
            {
                if (drive.IsReady && drive.DriveType.ToString() != "Removable" /*&& drive.DriveType.ToString() != "Network"*/) { disks.Add(new Disk(drive)); }
            }

            return disks;
        }
    }
}
