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

        /// <summary>
        /// array containing all relevent memory size for the disk
        /// </summary>
        private readonly static string[] MEMORY_SIZE = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <summary>
        /// The object containing the drive information
        /// </summary>
        public DriveInfo Drive { get; set; }

        /// <summary>
        /// the root letter of the drive
        /// </summary>
        public string Letter { get; private set; }

        /// <summary>
        /// object constructor of the the Disk class
        /// </summary>
        /// <param name="drive"></param>
        public Disk(DriveInfo drive)
        {
            Drive = drive;
            Letter = Drive.RootDirectory.Name;
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
        /// returns a string of the relevant information of the drive
        /// </summary>
        /// <returns>string containing the drive letter, percentage of used space and remaining space in readable format</returns>
        public override string ToString()
        {
            return Letter + new string(' ', 5) + (((float)Drive.TotalSize - Drive.TotalFreeSpace)/Drive.TotalSize*100).ToString("f").PadLeft(6) + "% " + GetReadableSize(Drive.TotalFreeSpace).PadLeft(10);
        }

        /// <summary>
        /// static method to print all the drives information in a table
        /// </summary>
        /// <param name="disks">list of disks to print information of</param>
        /// <param name="frame">the frame in which to print the disk information</param>
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
