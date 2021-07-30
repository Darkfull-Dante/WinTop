using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTop
{
    class Program
    {
        static void Main(string[] args)
        {
            UpdateUI();

            Console.ReadKey();
        }

        static void UpdateUI()
        {
            int x = Console.WindowWidth;
            int y = Console.WindowHeight;

            const int CPU_TABLE_HEIGHT = 10;

            //CPU usage
            Console.SetCursorPosition(0, 0);
            TitleLine("CPU Usage", x);
            PrintTableHeight(CPU_TABLE_HEIGHT);
        }

        static void TitleLine(string title, int width)
        {
            
            //write the start of the title line
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("┌─");

            //insert the title with a space buffer on each side
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" {0} ", title);

            //print an horizontal box drawing until end of window - 1
            Console.ForegroundColor = ConsoleColor.Blue;
            for (int i = Console.CursorLeft; i < width - 1; i++)
            {
                Console.Write('─');
            }

            //print the corner at the end of the window
            Console.Write('┐');

            //reset console color for next usage
            Console.ResetColor();
        }

        static void PrintTableHeight(int height)
        {
            int width = Console.WindowWidth - 1;
            int startHeight = Console.CursorTop;

            //set the color of the console text
            Console.ForegroundColor = ConsoleColor.Blue;

            for (int i = 0; i < height; i++)
            {
                Console.CursorTop = startHeight + i;
                
                Console.CursorLeft = 0;
                Console.Write('│');

                Console.CursorLeft = width;
                Console.Write('│');
            }

            Console.ResetColor();
        }
    }
}
