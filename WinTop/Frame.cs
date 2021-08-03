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

        private const char FRAME_VERTICAL = '│';
        private const char FRAME_HORIZONTAL = '─';
        private const char FRAME_TOP_LEFT = '╭';
        private const char FRAME_TOP_RIGHT = '╮';
        private const char FRAME_BOTTOM_LEFT = '╰';
        private const char FRAME_BOTTOM_RIGHT = '╯';

        /// <summary>
        /// Gets or sets the Width of the frame. Zero to take the rest of the space
        /// </summary>
        private int width;
        public int Width
        {
            get
            {
                if (width == 0)
                {
                    return Console.WindowWidth - PosX;
                }
                else
                {
                    return width;
                }
            }

            set { width = value; }
        }

        /// <summary>
        /// Gets or sets the height of the frame. Zero to take the rest of the sace
        /// </summary>
        private int height;
        public int Height
        {
            get
            {
                if (height == 0)
                {
                    return Console.WindowHeight - PosY;
                }
                else
                {
                    return height;
                }
            }
            set { height = value; }
        }

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

        public bool IsVisible { get; private set; }

        public int[] ProtectedData { get; set; }

        public Frame(int width, int height, string title, int posX, int posY, int minWidth, int minHeight)
        {
            Width = width;
            Height = height;
            Title = title;
            PosX = posX;
            PosY = posY;
            MinWidth = minWidth;
            MinHeight = minHeight;
            IsVisible = false;
            ProtectedData = new int[] { 0, 0 };
        }

        public static void UpdateFrame(List<Frame> frames)
        {
            bool valid;
            
            do
            {
                try
                {
                    int tempW = Console.WindowWidth;
                    int tempH = Console.WindowHeight;

                    if (tempW != windowWidth || tempH != windowHeight)
                    {
                        
                        //update the the new value
                        windowWidth = Console.WindowWidth;
                        windowHeight = Console.WindowHeight;

                        //clear the console
                        //Console.Clear();
                    }

                    foreach (Frame frame in frames)
                    {
                        frame.Draw();
                        frame.Clear();
                    }

                    valid = true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    valid = false;
                }
                
            } while (!valid);
            
            
        }

        public void Draw()
        {

            try
            {
                //check if available space for the frame
                if (windowWidth - 1 - PosX > MinWidth && (windowHeight) - 1 - PosY > MinHeight)
                {

                    IsVisible = true;


                    //print the title line
                    TitleLine();

                    //print the Column lines
                    ColumnPrint();

                    //print the last line
                    BottomLine();

                }
                else
                {
                    IsVisible = false;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }

            Console.SetWindowPosition(0, 0);
            
        }

        public void Clear()
        {
            if (IsVisible)
            {

                for (int i = PosY + 1; i < PosY + Height - 1; i++)
                {

                    int hStart = PosX + 1;
                    int stringWidth = Width - 2;
                    
                    if (i <= ProtectedData[1])
                    {
                        hStart += ProtectedData[0];
                        stringWidth -= ProtectedData[0];
                    }

                    Console.SetCursorPosition(hStart, i);
                    Console.Write(new string(' ', stringWidth));
                }
            }
            
        }

        private void BottomLine()
        {
            try
            {
                Console.SetCursorPosition(PosX, PosY + Height - 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            
            Console.ForegroundColor = FRAME_COLOR;
            Console.Write("{0}{1}{2}", FRAME_BOTTOM_LEFT, new string(FRAME_HORIZONTAL, Width - 2), FRAME_BOTTOM_RIGHT);
            Console.ResetColor();
        }

        private void ColumnPrint()
        {
            int endH = PosY + Height;
            int endW = PosX + Width - 1;

            try
            {
                Console.ForegroundColor = FRAME_COLOR;
                Console.SetCursorPosition(PosX, PosY + 1);

                for (int i = Console.CursorTop; i < endH - 1; i++)
                {
                    Console.SetCursorPosition(PosX, i);
                    Console.Write(FRAME_VERTICAL);
                    Console.CursorLeft = endW;
                    Console.Write(FRAME_VERTICAL);
                }

                Console.ResetColor();
            }
            catch (Exception)
            {

                throw;
            }
            

        }

        private void TitleLine()
        {

            try
            {
                int endW = PosX + Width;

                Console.SetCursorPosition(PosX, PosY);

                //pre-title characters
                Console.ForegroundColor = FRAME_COLOR;
                Console.Write("{0}{1}", FRAME_TOP_LEFT, FRAME_HORIZONTAL);

                //print the title if not null
                if (!string.IsNullOrEmpty(Title))
                {
                    Console.ForegroundColor = TITLE_COLOR;
                    Console.Write(" {0} ", Title);
                }

                //print until end of the Frame - 1
                Console.ForegroundColor = FRAME_COLOR;
                for (int i = Console.CursorLeft; i < endW - 1; i++)
                {
                    Console.Write(FRAME_HORIZONTAL);
                }

                //print last corner of Frame title line
                Console.Write(FRAME_TOP_RIGHT);
                Console.ResetColor();
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            
        }

    }
}
