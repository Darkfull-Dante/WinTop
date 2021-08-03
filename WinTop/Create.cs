using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WinTop.Components;
using WinTop.Graphics;
using System.IO;

namespace WinTop
{

    class Create
    {

        public const int CPU_FRAME = 0;
        public const int DSK_FRAME = 1;
        public const int TMP_FRAME = 2;
        public const int MEM_FRAME = 3;
        public const int PRC_FRAME = 4;
        public const int NTW_FRAME = 5;

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
            frames.Add(new Frame(0, 14, "Memory Usage", 30, 12, 18, 14));
            frames.Add(new Frame(50, 13, "Processes", 0, 26, 50, 13));
            frames.Add(new Frame(0, 13, "Network Usage", 50, 26, 19, 13));

            return frames;
        }

        public static List<CPU> CPUCores()
        {
            int coreCount = Environment.ProcessorCount;


            List<CPU> cpuCores = new List<CPU>();

            for (int i = 0; i < coreCount; i++)
            {
                cpuCores.Add(new CPU(new PerformanceCounter("Processor", "% Processor Time", i.ToString()), new Chart(Program.appFrames[CPU_FRAME], CPU.CPUColor(i), CPU_FRAME), CPU_FRAME));
            }

            return cpuCores;
        }

        public static List<Disk> Disks()
        {
            DriveInfo[] driveInfos = DriveInfo.GetDrives();
            List<Disk> disks = new List<Disk>();

            foreach (DriveInfo drive in driveInfos)
            {
                disks.Add(new Disk(drive));
            }

            return disks;
        }
    }
}
