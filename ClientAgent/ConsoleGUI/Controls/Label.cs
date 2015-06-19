// --------------------------------------------------------------
// <copyright file="Label.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="Label"/> class.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ConsoleGUI.IO;

    /// <summary>
    /// Provides a label control.
    /// </summary>
    public class Label : IRenderable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="renderer">
        ///     The renderer used for this label.
        /// </param>
        public Label(IRenderer renderer)
        {
            this.Renderer = renderer;
            this.Text = "Label";
            this.Rectangle = new Rectangle(0, 0, 8, 1);
            this.BackgroundColor = ConsoleColor.DarkBlue;
            this.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Gets the renderer for this label.
        /// </summary>
        /// <value>
        ///     Contains the renderer for this label.
        /// </value>
        public IRenderer Renderer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the label's background color.
        /// </summary>
        /// <value>
        ///     Contains the label's background color.
        /// </value>
        public ConsoleColor BackgroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the label's foreground color.
        /// </summary>
        /// <value>
        ///     Contains the label's foreground color.
        /// </value>
        public ConsoleColor ForegroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text of the label.
        /// </summary>
        /// <value>
        ///     Contains the text of the label.
        /// </value>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the label is visible.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the label is visible.
        /// </value>
        public bool Visible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the z position of the label.
        /// </summary>
        /// <value>
        ///     Contains the z position of the label.
        /// </value>
        public int Z
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rectangle of the label.
        /// </summary>
        /// <value>
        ///     Contains the rectangle of the label.
        /// </value>
        public Rectangle Rectangle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the pixels of the label.
        /// </summary>
        /// <returns>
        ///     Returns a 2-dimensional <see cref="Pixel"/> array containing the rendered pixels.
        /// </returns>
        public Pixel[,] GetPixels()
        {
            Pixel[,] ret = new Pixel[this.Rectangle.Width, this.Rectangle.Height];
            int mid = this.Rectangle.Height / 2;

            for (int y = 0; y < this.Rectangle.Height; y++)
            {
                if (y == mid)
                {
                    continue;
                }

                for (int x = 0; x < this.Rectangle.Width; x++)
                {
                    ret[x, y] = new Pixel(this.BackgroundColor, this.ForegroundColor, ' ');
                }
            }
            
            // line with text
            for (int x = 1; x < this.Rectangle.Width - 1; x++)
            {
                char c = ' ';
                if (x - 1 < this.Text.Length)
                {
                    c = this.Text[x - 1];
                }

                ret[x, mid] = new Pixel(this.BackgroundColor, this.ForegroundColor, c);
            }

            ret[0, mid] = new Pixel(this.BackgroundColor, this.ForegroundColor, ' ');
            ret[this.Rectangle.Width - 1, mid] = new Pixel(this.BackgroundColor, this.ForegroundColor, ' ');

            return ret;
        }
    }
}
