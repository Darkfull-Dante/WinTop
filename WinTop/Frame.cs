using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTop
{
    class Frame
    {

        private static int windowWidth;
        private static int windowHeight;
        private const ConsoleColor FRAME_COLOR = ConsoleColor.Cyan;
        private const ConsoleColor TITLE_COLOR = ConsoleColor.White;

        /// <summary>
        /// Gets or sets the Width of the frame. Zero to take the rest of the space
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the frame. Zero to take the rest of the sace
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the Title of the frame. Null for no title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the horizontal start position of the frame.
        /// </summary>
        public int PosX { get; set; }

        /// <summary>
        /// Gets or sets the vertical start position of the frame.
        /// </summary>
        public int PosY { get; set; }

        /// <summary>
        /// Gets or sets the minimum Width for the frame to appears.
        /// </summary>
        public int MinWidth { get; set; }

        /// <summary>
        /// Gets or sets the minimum height for the frame to appears.
        /// </summary>
        public int MinHeight { get; set; }

        public Frame(int width, int height, string title, int posX, int posY, int minWidth, int minHeight)
        {
            Width = width;
            Height = height;
            Title = title;
            PosX = posX;
            PosY = posY;
            MinWidth = minWidth;
            MinHeight = minHeight;
        }

        public static void UpdateFrame(List<Frame> frames)
        {
            int tempW = Console.WindowWidth;
            int tempH = Console.WindowHeight;

            //check if console window changed size
            if (windowWidth != Console.WindowWidth || windowHeight != Console.WindowHeight)
            {
                //wait
                System.Threading.Thread.Sleep(1000);

                //update the the new value
                windowWidth = Console.WindowWidth;
                windowHeight = Console.WindowHeight;

                //clear the console
                Console.Clear();

                //prints the Frame
                foreach (Frame frame in frames)
                {
                    //check if available space for the frame
                    if (windowWidth - 1 - frame.PosX > frame.MinWidth && windowHeight - 1 - frame.PosY > frame.MinHeight)
                    {
                        //print the title line
                        frame.TitleLine();

                        //print the Column lines
                        frame.ColumnPrint();

                        //print the last line
                        frame.BottomLine();
                    }
                }
            }
        }

        private void BottomLine()
        {
            int width = (Width == 0) ? (Console.WindowWidth - PosX) : Width;

            Console.SetCursorPosition(PosX, PosY + Height - 1);
            Console.ForegroundColor = FRAME_COLOR;
            Console.Write("└{0}┘", new string('─', width - 2));
            Console.ResetColor();
        }

        private void ColumnPrint()
        {
            int endH = (Height == 0) ? Console.WindowHeight - 1 : (PosY + Height);
            int endW = (Width == 0) ? Console.WindowWidth - 1 : (PosX + Width - 1);

            Console.ForegroundColor = FRAME_COLOR;
            Console.SetCursorPosition(PosX, PosY + 1);

            for (int i = Console.CursorTop; i < endH - 1; i++)
            {
                Console.SetCursorPosition(PosX, i);
                Console.Write('│');
                Console.CursorLeft = endW;
                Console.Write('│');
            }

            Console.ResetColor();

        }

        private void TitleLine()
        {

            int endW = (Width == 0) ? Console.WindowWidth : (PosX + Width);

            Console.SetCursorPosition(PosX, PosY);
            
            //pre-title characters
            Console.ForegroundColor = FRAME_COLOR;
            Console.Write("┌─");
            
            //print the title if not null
            if(!string.IsNullOrEmpty(Title))
            {
                Console.ForegroundColor = TITLE_COLOR;
                Console.Write(" {0} ", Title);
            }

            //print until end of the Frame - 1
            Console.ForegroundColor = FRAME_COLOR;
            for (int i = Console.CursorLeft; i < endW - 1; i++)
            {
                Console.Write('─');
            }

            //print last corner of Frame title line
            Console.Write('┐');
            Console.ResetColor();
        }

    }
}
