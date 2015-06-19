// --------------------------------------------------------------
// <copyright file="Pixel.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="Pixel"/> class.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides information about a pixel.
    /// </summary>
    public class Pixel
    {
        /// <summary>
        /// The value for the <see cref="BackgroundColor"/> property.
        /// </summary>
        private ConsoleColor backgroundColorProperty;

        /// <summary>
        /// The value for the <see cref="ForegroundColor"/> property.
        /// </summary>
        private ConsoleColor foregroundColorProperty;

        /// <summary>
        /// The value for the <see cref="Char"/> property.
        /// </summary>
        private char charProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pixel"/> class.
        /// </summary>
        public Pixel()
        {
            this.BackgroundColor = ConsoleColor.Black;
            this.ForegroundColor = ConsoleColor.Black;
            this.Char = '\0';
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pixel"/> class.
        /// </summary>
        /// <param name="bgc">
        ///     The background color of the pixel.
        /// </param>
        /// <param name="fgc">
        ///     The foreground color of the pixel.
        /// </param>
        /// <param name="c">
        ///     The character of the pixel.
        /// </param>
        public Pixel(ConsoleColor bgc, ConsoleColor fgc, char c)
        {
            this.BackgroundColor = bgc;
            this.ForegroundColor = fgc;
            this.Char = c;
        }

        /// <summary>
        /// Raised when a property of the pixel was modified.
        /// </summary>
        public event EventHandler PixelModified;

        /// <summary>
        /// Gets or sets the background color of the pixel.
        /// </summary>
        /// <value>
        ///     Contains the background color of the pixel.
        /// </value>
        public ConsoleColor BackgroundColor
        {
            get
            {
                return this.backgroundColorProperty;
            }

            set
            {
                this.backgroundColorProperty = value;
                this.OnPixelModified(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the pixel.
        /// </summary>
        /// <value>
        ///     Contains the foreground color of the pixel.
        /// </value>
        public ConsoleColor ForegroundColor
        {
            get
            {
                return this.foregroundColorProperty;
            }

            set
            {
                this.foregroundColorProperty = value;
                this.OnPixelModified(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the character of the pixel.
        /// </summary>
        /// <value>
        ///     Contains the character of the pixel.
        /// </value>
        public char Char
        {
            get
            {
                return this.charProperty;
            }

            set
            {
                this.charProperty = value;
                this.OnPixelModified(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Loads a pixel from a string.
        /// </summary>
        /// <param name="px">
        ///     The string that contains information about the pixel.
        /// </param>
        /// <param name="x">
        ///     The x coordinate of the pixel.
        /// </param>
        /// <param name="y">
        ///     The y coordinate of the pixel.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="Pixel"/> instance built from the specified string.
        /// </returns>
        /// <exception cref="FormatException">
        ///     Thrown when the passed string does not follow the format rules.
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        ///     Thrown when the read x and/or y coordinate is out of range.
        /// </exception>
        public static Pixel LoadFromString(string px, out int x, out int y)
        {
            Pixel p = new Pixel();

            // x, y, fgc, bgc, char
            string[] spl = px.Split(',');

            x = -1;
            y = -1;

            int fg = -1, bg = -1;

            bool ok = spl.Length == 5 &&
                      int.TryParse(spl[0], out x) &&
                      int.TryParse(spl[1], out y) &&
                      int.TryParse(spl[2], out fg) &&
                      int.TryParse(spl[3], out bg) &&
                      spl[4].Length == 1 &&
                      (char.IsLetterOrDigit(spl[4][0]) || spl[4][0] == '\0');

            // check exceptions
            if (!ok)
            {
                throw new FormatException("The passed string does not follow the format rules: x, y, foreground (int), background (int), char (digit or letter).");
            }

            if (x < 0)
            {
                throw new IndexOutOfRangeException("The passed x coordinate is smaller than 0.");
            }

            if (y < 0)
            {
                throw new IndexOutOfRangeException("The passed y coordinate is smaller than 0.");
            }

            p.BackgroundColor = (ConsoleColor)fg;
            p.ForegroundColor = (ConsoleColor)bg;
            p.Char = spl[4][0];

            return p;
        }

        /// <summary>
        /// Raises the <see cref="PixelModified"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnPixelModified(EventArgs e)
        {
            if (this.PixelModified != null)
            {
                this.PixelModified(this, e);
            }
        }
    }
}
