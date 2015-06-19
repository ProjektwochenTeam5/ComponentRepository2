// --------------------------------------------------------------
// <copyright file="Rectangle.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="Rectangle"/> struct.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI
{
    using System;

    /// <summary>
    /// Provides information about a rectangle.
    /// </summary>
    public struct Rectangle
    {
        /// <summary>
        /// Contains the value for the <see cref="Width"/> property.
        /// </summary>
        private int widthProperty;

        /// <summary>
        /// Contains the value for the <see cref="Height"/> property.
        /// </summary>
        private int heightProperty;

        /// <summary>
        /// Contains the value for the <see cref="X"/> property.
        /// </summary>
        private int xproperty;

        /// <summary>
        /// Contains the value for the <see cref="Y"/> property.
        /// </summary>
        private int yproperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="x">
        ///     The x coordinate of the rectangle.
        /// </param>
        /// <param name="y">
        ///     The y coordinate of the rectangle.
        /// </param>
        /// <param name="w">
        ///     The width of the rectangle.
        /// </param>
        /// <param name="h">
        ///     The height of the rectangle.
        /// </param>
        public Rectangle(int x, int y, int w, int h)
        {
            this.widthProperty = 0;
            this.heightProperty = 0;
            this.xproperty = x;
            this.yproperty = y;
            this.X = x;
            this.Y = y;
            this.Width = w;
            this.Height = h;
        }

        /// <summary>
        /// Gets the x coordinate of the rectangle.
        /// </summary>
        /// <value>
        ///     Contains the x coordinate of the rectangle.
        /// </value>
        public int X
        {
            get
            {
                return this.xproperty;
            }

            private set
            {
                this.xproperty = value;
            }
        }

        /// <summary>
        /// Gets the y coordinate of the rectangle.
        /// </summary>
        /// <value>
        ///     Contains the y coordinate of the rectangle.
        /// </value>
        public int Y
        {
            get
            {
                return this.yproperty;
            }

            private set
            {
                this.yproperty = value;
            }
        }

        /// <summary>
        /// Gets the width of the rectangle.
        /// </summary>
        /// <value>
        ///     Contains the width of the rectangle.
        /// </value>
        public int Width
        {
            get
            {
                return this.widthProperty;
            }

            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("The value must be greater or equals 0.");
                }

                this.widthProperty = value;
            }
        }

        /// <summary>
        /// Gets the height of the rectangle.
        /// </summary>
        /// <value>
        ///     Contains the height of the rectangle.
        /// </value>
        public int Height
        {
            get
            {
                return this.heightProperty;
            }

            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("The value must be greater or equals 0.");
                }

                this.heightProperty = value;
            }
        }

        /// <summary>
        /// Gets the total range of the rectangle on the x axis.
        /// </summary>
        /// <value>
        ///     Contains the total range of the rectangle on the x axis.
        /// </value>
        public int RangeX
        {
            get
            {
                return this.X + this.Width;
            }
        }

        /// <summary>
        /// Gets the total range of the rectangle on the y axis.
        /// </summary>
        /// <value>
        ///     Contains the total range of the rectangle on the y axis.
        /// </value>
        public int RangeY
        {
            get
            {
                return this.Y + this.Height;
            }
        }
    }
}
