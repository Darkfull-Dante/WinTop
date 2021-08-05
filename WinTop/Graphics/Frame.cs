using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTop.Graphics
{
    class Frame
    {

        /// <summary>
        /// the console color for the frame itself
        /// </summary>
        private const ConsoleColor FRAME_COLOR = ConsoleColor.Cyan;

        /// <summary>
        /// the console color for the title
        /// </summary>
        private const ConsoleColor TITLE_COLOR = ConsoleColor.White;

        /// <summary>
        /// the character for a vertical line in the frame
        /// </summary>
        private const char FRAME_VERTICAL = '│';

        /// <summary>
        /// the character for an horizontal line in the frame
        /// </summary>
        private const char FRAME_HORIZONTAL = '─';

        /// <summary>
        /// the character for a top left corner in the frame
        /// </summary>
        private const char FRAME_TOP_LEFT = '╭';

        /// <summary>
        /// the character for a top right corner in the frame
        /// </summary>
        private const char FRAME_TOP_RIGHT = '╮';

        /// <summary>
        /// the character for a bottom left corner in the frame
        /// </summary>
        private const char FRAME_BOTTOM_LEFT = '╰';

        /// <summary>
        /// the character for bottom right corner in the frame
        /// </summary>
        private const char FRAME_BOTTOM_RIGHT = '╯';

        /// <summary>
        /// Width of the frame
        /// </summary>
        public int Width
        {
            get
            {
                if (width == 0)
                {
                    return Program.screenBuffer.Width - PosX;
                }
                else
                {
                    return width;
                }
            }

            set { width = value; }
        }
        private int width;

        /// <summary>
        /// Height of the frame
        /// </summary>
        public int Height
        {
            get
            {
                if (height == 0)
                {
                    return Program.screenBuffer.Height - PosY;
                }
                else
                {
                    return height;
                }
            }
            set { height = value; }
        }
        private int height;

        /// <summary>
        /// Title of the frame. If empty, no title is printed
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// starting position on the x axis of the frame
        /// </summary>
        public int PosX { get; set; }

        /// <summary>
        /// starting position on the y axis of the frame
        /// </summary>
        public int PosY { get; set; }

        /// <summary>
        /// Minimum width needed to print the frame
        /// </summary>
        public int MinWidth { get; set; }

        /// <summary>
        /// Minimum heght nedded to print the frame
        /// </summary>
        public int MinHeight { get; set; }

        /// <summary>
        /// boolean informing if the frame is currently visible
        /// </summary>
        public bool IsVisible { get; private set; }

        /// <summary>
        /// array containing {width, height} value of the protected data inside the frame
        /// </summary>
        public int[] ProtectedData { get; set; }

        /// <summary>
        /// object of the constructor of the Frame class
        /// </summary>
        /// <param name="width">Width of the frame</param>
        /// <param name="height">Height of the frame</param>
        /// <param name="title">Title of the frame</param>
        /// <param name="posX">starting position on the x axis of the frame</param>
        /// <param name="posY">starting position on the y axis of the frame</param>
        /// <param name="minWidth">Minimum width needed to print the frame</param>
        /// <param name="minHeight">Minimum heght nedded to print the frame</param>
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

        /// <summary>
        /// updates a list of frames and prints them to the screen Buffer
        /// </summary>
        /// <param name="frames">a list of frame object</param>
        public static void UpdateFrame(List<Frame> frames)
        {
            bool valid;

            Console.CursorVisible = false;

            do
            {
                try
                {
                    if (Program.screenBuffer.Width != Console.WindowWidth || Program.screenBuffer.Height != Console.WindowHeight)
                    {
                        //update screenBuffer Width and Height
                        Program.screenBuffer.UpdateBufferSize();
                    }

                    foreach (Frame frame in frames)
                    {
                        frame.Draw();
                        frame.Clear();
                    }

                    valid = true;
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentOutOfRangeException || ex is IndexOutOfRangeException)
                    {
                        Program.screenBuffer.UpdateBufferSize();
                        valid = false;

                    }
                    else
                    {
                        throw;
                    }
                }
            }
            while (!valid);
        }

        /// <summary>
        /// draws the frame on the screen buffer
        /// </summary>
        public void Draw()
        {

            try
            {
                //check if available space for the frame
                if (Program.screenBuffer.Width - 1 - PosX > MinWidth && Program.screenBuffer.Height - 1 - PosY > MinHeight)
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
            catch (IndexOutOfRangeException)
            {
                throw;
            }
            
        }

        /// <summary>
        /// clears the content area of the frame
        /// </summary>
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

                    try
                    {
                        Program.screenBuffer.SetCursorPosition(hStart, i);
                        Program.screenBuffer.Write(new string(' ', stringWidth));
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        throw;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw;
                    }
                    
                }
            }
            
        }

        /// <summary>
        /// prints the bottom line of the frame
        /// </summary>
        private void BottomLine()
        {
            try
            {
                Program.screenBuffer.SetCursorPosition(PosX, PosY + Height - 1);
            }
            catch (IndexOutOfRangeException)
            {
                throw;
            }

            Program.screenBuffer.Write(string.Format("{0}{1}{2}", FRAME_BOTTOM_LEFT, new string(FRAME_HORIZONTAL, Width - 2), FRAME_BOTTOM_RIGHT), FRAME_COLOR);
        }

        /// <summary>
        /// prints the columns of the frame
        /// </summary>
        private void ColumnPrint()
        {
            int endH = PosY + Height;
            int endW = PosX + Width - 1;

            try
            {
                Program.screenBuffer.SetCursorPosition(PosX, PosY + 1);

                for (int i = Program.screenBuffer.CursorTop; i < endH - 1; i++)
                {
                    Program.screenBuffer.SetCursorPosition(PosX, i);
                    Program.screenBuffer.Write(FRAME_VERTICAL, FRAME_COLOR);
                    Program.screenBuffer.CursorLeft = endW;
                    Program.screenBuffer.Write(FRAME_VERTICAL, FRAME_COLOR);
                }
            }
            catch (IndexOutOfRangeException)
            {

                throw;
            }
            

        }

        /// <summary>
        /// prints the title line of the frame
        /// </summary>
        private void TitleLine()
        {

            try
            {
                int endW = PosX + Width;

                Program.screenBuffer.SetCursorPosition(PosX, PosY);

                //pre-title characters
                Program.screenBuffer.Write(string.Format("{0}{1}", FRAME_TOP_LEFT, FRAME_HORIZONTAL), FRAME_COLOR);

                //print the title if not null
                if (!string.IsNullOrEmpty(Title))
                {
                    Program.screenBuffer.Write(string.Format(" {0} ", Title), TITLE_COLOR);
                }

                //print until end of the Frame - 1
                for (int i = Program.screenBuffer.CursorLeft; i < endW - 1; i++)
                {
                    Program.screenBuffer.Write(FRAME_HORIZONTAL, FRAME_COLOR);
                }

                //print last corner of Frame title line
                Program.screenBuffer.Write(FRAME_TOP_RIGHT, FRAME_COLOR);
            }
            catch (IndexOutOfRangeException)
            {
                throw;
            }
            
        }

    }
}
