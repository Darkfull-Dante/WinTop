using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTop.Graphics
{
    /// <summary>
    /// A class for managing a second screen buffer for the console. This screen buffer implements some of the most usefull methods of the console class to try an mimic it in the most practical way possible
    /// </summary>
    class ScreenBuffer
    {

        /// <summary>
        /// the size (in spaces) of a tab for the screen buffer
        /// </summary>
        public const int TAB_SIZE = 8;
        
        /// <summary>
        /// a 2D array of character representing the futur text of the console window
        /// </summary>
        public char[,] CharArray { get; set; }

        /// <summary>
        /// a 2D array of console color representing the colors to apply on the futur text of the console window
        /// </summary>
        public ConsoleColor[,] ColorArray { get; set; }

        /// <summary>
        /// Width of the screen buffer
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the screen buffer
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Current position of the Cursor from the left of the screen buffer (zero-based)
        /// </summary>
        public int CursorLeft { get; set; }

        /// <summary>
        /// Current position of the Cursor from the top of the screen buffer (zero-based)
        /// </summary>
        public int CursorTop { get; set; }

        /// <summary>
        /// object constructor of the ScreenBuffer class
        /// </summary>
        /// <param name="charArray">a 2D array of character representing the futur text of the console window</param>
        /// <param name="colorArray">a 2D array of console color representing the colors to apply on the futur text of the console window</param>
        public ScreenBuffer(char[,] charArray, ConsoleColor[,] colorArray)
        {
            CharArray = charArray;
            ColorArray = colorArray;
            Width = CharArray.GetLength(0);
            Height = CharArray.GetLength(1);
            CursorLeft = 0;
            CursorTop = 0;
        }
        
        /// <summary>
        /// object constructor of the ScreenBuffer class
        /// </summary>
        /// <param name="width">Width of the screen buffer</param>
        /// <param name="height">Height of the screen buffer</param>
        public ScreenBuffer(int width, int height) : this(new char[width, height], new ConsoleColor[width, height]) { }

        /// <summary>
        /// writes text to the screen buffer with the current color of the console foreground
        /// </summary>
        /// <param name="text">a string of text</param>
        public void Write(string text)
        {
            try
            {
                Write(text, Console.ForegroundColor);
            }
            catch (IndexOutOfRangeException) { throw; }
            
        }

        /// <summary>
        /// writes a character to the screen buffer with the current color of the console foreground
        /// </summary>
        /// <param name="c">character to print</param>
        public void Write(char c)
        {
            try
            {
                Write(c, Console.ForegroundColor);
            }
            catch (IndexOutOfRangeException) { throw; }
            
        }

        /// <summary>
        /// writes a character to the screen buffer
        /// </summary>
        /// <param name="c">character to print</param>
        /// <param name="color">color of the text</param>
        public void Write(char c, ConsoleColor color)
        {
            switch (c)
            {
                case '\n':
                    SetCursorPosition(0, CursorTop++);
                    break;
                case '\t':
                    
                    //get the shift needed to get to the next tab
                    int shift = CursorLeft % TAB_SIZE;

                    //if already on a tab, shift to the next
                    if (shift == 0)
                    {
                        shift = TAB_SIZE;
                    }

                    //set the cursor if not out of bound
                    if (CursorLeft + shift < Width)
                    {
                        CursorLeft += shift;
                    }
                    break;
                default:
                    try
                    {
                        CharArray[CursorLeft, CursorTop] = c;
                        ColorArray[CursorLeft, CursorTop] = color;
                        CursorLeft++;
                    }
                    catch (IndexOutOfRangeException)
                    {

                        throw;
                    }
                    
                    break;
            }
        }

        /// <summary>
        /// writes text to the screen buffer
        /// </summary>
        /// <param name="text">a string of text</param>
        /// <param name="color">color of the text</param>
        public void Write(string text, ConsoleColor color)
        {
            for (int i = 0; i < text.Length; i++)
            {

                try
                {
                    Write(text[i], color);
                    
                }
                catch (IndexOutOfRangeException)
                {
                    throw;
                }
                
            }
        }

        /// <summary>
        /// set the cursor to the x:y position in the screen buffer 
        /// </summary>
        /// <param name="x">horizontal position</param>
        /// <param name="y">vertical position</param>
        public void SetCursorPosition(int x, int y)
        {
            CursorLeft = x;
            CursorTop = y;
        }

        /// <summary>
        /// writes text to the screen buffer with a new line character at the end
        /// </summary>
        /// <param name="text">a string of text</param>
        /// <param name="color">color of the text</param>
        public void WriteLine(string text, ConsoleColor color)
        {

            text += "\n";
            
            try
            {
                Write(text, color);
            }
            catch (IndexOutOfRangeException)
            {

                throw;
            }
        }

        /// <summary>
        /// updates the size of the screen buffer to the current size of the console window size
        /// </summary>
        public void UpdateBufferSize()
        {
            UpdateBufferSize(Console.WindowWidth, Console.WindowHeight - 1);
        }

        /// <summary>
        /// updates the size of the screen buffer to the width and height provided
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void UpdateBufferSize(int width, int height)
        {
            Width = width;
            Height = height;

            //resize charArray
            CharArray = ResizeArray(CharArray, width, height);
            ColorArray = ResizeArray(ColorArray, width, height);
        }

        /// <summary>
        /// Resizes a 2 dimensional array
        /// </summary>
        /// <typeparam name="T">the type of value the array olds</typeparam>
        /// <param name="original">the original array</param>
        /// <param name="width">new width of the array</param>
        /// <param name="height">new height of the array</param>
        /// <returns>resized array</returns>
        private T[,] ResizeArray<T>(T[,] original, int width, int height)
        {
            var newArray = new T[width, height];
            int minRows = Math.Min(width, original.GetLength(0));
            int minCols = Math.Min(height, original.GetLength(1));
            for (int i = 0; i < minRows; i++)
                for (int j = 0; j < minCols; j++)
                    newArray[i, j] = original[i, j];
            return newArray;
        }

        /// <summary>
        /// clears the screen buffer
        /// </summary>
        public void Clear()
        {
            CharArray = new char[Width, Height];
            ColorArray = new ConsoleColor[Width, Height];
        }

        /// <summary>
        /// prints the screen buffer
        /// </summary>
        public void Print()
        {
            try
            {
                Console.SetCursorPosition(0, Height);
                Console.Write(new string(' ', Width - 1));

                for (int i = 0; i < Console.WindowHeight; i++)
                {
                    for (int j = 0; j < Console.WindowWidth; j++)
                    {
                        Console.SetCursorPosition(j, i);
                        Console.ForegroundColor = ColorArray[j, i];
                        Console.Write(CharArray[j, i]);

                    }
                }

                Console.ResetColor();
            }
            catch (IndexOutOfRangeException) { throw; }
        }
    }
}
