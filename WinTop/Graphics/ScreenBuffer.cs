using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTop.Graphics
{
    class ScreenBuffer
    {
        public char[,] CharArray { get; set; }
        public ConsoleColor[,] ColorArray { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int CursorLeft { get; set; }
        public int CursorTop { get; set; }

        public ScreenBuffer(char[,] charArray, ConsoleColor[,] colorArray)
        {
            CharArray = charArray;
            ColorArray = colorArray;
            Width = CharArray.GetLength(0);
            Height = CharArray.GetLength(1);
            CursorLeft = 0;
            CursorTop = 0;
        }

        public ScreenBuffer(int width, int height) : this(new char[width, height], new ConsoleColor[width, height]) { }

        public void Write(string text)
        {
            try
            {
                Write(text, Console.ForegroundColor);
            }
            catch (IndexOutOfRangeException) { throw; }
            
        }

        public void Write(char c)
        {
            try
            {
                Write(c, Console.ForegroundColor);
            }
            catch (IndexOutOfRangeException) { throw; }
            
        }

        public void Write(char c, ConsoleColor color)
        {
            switch (c)
            {
                case '\n':
                    SetCursorPosition(0, CursorTop++);
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

        public void SetCursorPosition(int x, int y)
        {
            CursorLeft = x;
            CursorTop = y;
        }

        public void WriteLine(string text, ConsoleColor color)
        {
            try
            {
                Write(text, color);
            }
            catch (IndexOutOfRangeException)
            {

                throw;
            }
            SetCursorPosition(0, CursorTop);
        }

        public void UpdateBufferSize()
        {
            UpdateBufferSize(Console.WindowWidth, Console.WindowHeight - 1);
        }

        public void UpdateBufferSize(int width, int height)
        {
            Width = width;
            Height = height;

            //resize charArray
            CharArray = ResizeArray(CharArray, width, height);
            ColorArray = ResizeArray(ColorArray, width, height);
        }

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

        public void Clear()
        {
            CharArray = new char[Width, Height];
            ColorArray = new ConsoleColor[Width, Height];
        }

        public void Print()
        {
            //try
            //{
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
            //}
            //catch (IndexOutOfRangeException)
            //{

                //throw;
            //}
            
        }
    }
}
