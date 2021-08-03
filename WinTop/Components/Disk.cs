using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using WinTop.Graphics;

namespace WinTop.Components
{
    class Disk
    {

        private readonly static string[] MEMORY_SIZE = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        private DriveInfo Drive { get; set; }
        public string Letter { get; set; }

        public Disk(DriveInfo drive)
        {
            Drive = drive;
            Letter = Drive.RootDirectory.Name;
        }

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

        public override string ToString()
        {
            return Letter + new string(' ', 5) + ((float)Drive.TotalFreeSpace/Drive.TotalSize*100).ToString("f").PadLeft(6) + "% " + GetReadableSize(Drive.TotalFreeSpace).PadLeft(10);
        }

        public static void Print(List<Disk> disks, Frame frame)
        {
            //check max number of drive
            int maxNumberOfDrives = Math.Min(frame.Height - 3, disks.Count);
            int hStart = frame.PosX + 1;
            int vStart = frame.PosY + 1;

            //print the title
            Program.screenBuffer.SetCursorPosition(hStart, vStart++);
            Program.screenBuffer.Write("Letter  Used     Free", ConsoleColor.White);

            //print each drive info
            for (int i = 0; i < maxNumberOfDrives; i++)
            {
                Program.screenBuffer.SetCursorPosition(hStart, vStart + i);
                Program.screenBuffer.Write(disks[i].ToString());
            }
        }

    }
}
