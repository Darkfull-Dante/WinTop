using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTop
{
    class Frame
    {
        private int width;
        private int height;
        private string title;
        private int usableWidth;
        private int usableHeight;
        private string relativeWidth;
        private string relativeHeight;
        private int posX;
        private int posY;

        public static int windowWidth;
        public static int windowHeight;

        /// <summary>
        /// Horizontal start position (from left) of the Frame.
        /// </summary>
        public int PosX
        {
            get { return posX; }
            set { posX = value; }
        }

        /// <summary>
        /// Vertical start position (from top) of the Frame.
        /// </summary>
        public int PosY
        {
            get { return posY; }
            set { posY = value; }
        }

        /// <summary>
        /// width of the frame. Cancels the relativeWidth if set.
        /// </summary>
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                usableWidth = Width - 4;
                relativeWidth = null;
            }
        }

        /// <summary>
        /// height of the frame. Cancels the relativeHeight if set.
        /// </summary>
        public int Height
        {
            get { return height; }
            set
            {
                width = value;
                usableHeight = Height - 4;
                relativeHeight = null;
            }
        }

        /// <summary>
        /// the available width for content of the frame.
        /// </summary>
        public int UsableWidth
        {
            get { return usableWidth; }
            private set { usableWidth = value; }
        }

        /// <summary>
        /// the available height for content of the frame.
        /// </summary>
        public int UsableHeight
        {
            get { return usableHeight; }
            private set { usableHeight = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// function that validates a 'star value' is correctly formated and returns the number of 'star as an int
        /// </summary>
        /// <param name="starValue">a star value using the 'x*' standard (an int x followed by a star char) that represents the shape should take x equal parts of the remaining space</param>
        /// <returns>the number of equal parts the shape should take.</returns>
        private int IsValidStarValue(string starValue)
        {
            //validate last char if the string is a star
            if (starValue[starValue.Length - 1] != '*')
            {
                return 0;
            }
            else if (starValue.Length == 1)
            {
                return 1;
            }

            //remove the star and convert the remaining string to an int
            try
            {
                return int.Parse(starValue.Substring(0, starValue.Length - 1));
            }
            catch (Exception)
            {
                return 0;
            }
        }

    }
}
