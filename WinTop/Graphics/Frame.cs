using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTop.Graphics
{
    class Frame
    {

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
                    return Program.screenBuffer.Width - PosX;
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
                    return Program.screenBuffer.Height - PosY;
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
