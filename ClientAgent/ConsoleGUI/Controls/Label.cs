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
    using ConsoleGUI;
    using ConsoleGUI.IO;

    /// <summary>
    /// Provides a label control.
    /// </summary>
    public class Label : Control
    {
        /// <summary>
        /// The value for the <see cref="Text"/> property.
        /// </summary>
        private string textProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="renderers">
        ///     The renderer used for this label.
        /// </param>
        public Label(ICollection<IRenderer> renderers) : base(renderers)
        {
            this.Text = "Label";
            this.Rectangle = new Rectangle(0, 0, 8, 1);
            this.BackgroundColor = ConsoleColor.DarkBlue;
            this.ForegroundColor = ConsoleColor.White;
            this.TextChanged += this.Label_TextChanged;
        }

        /// <summary>
        /// Raised when the <see cref="Text"/> property was changed.
        /// </summary>
        public event EventHandler TextChanged;

        /// <summary>
        /// Gets or sets the text of the label.
        /// </summary>
        /// <value>
        ///     Contains the text of the label.
        /// </value>
        public string Text
        {
            get
            {
                return this.textProperty;
            }

            set
            {
                this.textProperty = value;
                this.OnTextChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the pixels of the label.
        /// </summary>
        /// <returns>
        ///     Returns a 2-dimensional <see cref="Pixel"/> array containing the rendered pixels.
        /// </returns>
        public override Pixel[,] GetPixels()
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

        /// <summary>
        /// Sends a key to the label.
        /// </summary>
        /// <param name="k">
        ///     The key that shall be sent.
        /// </param>
        /// <returns>
        ///     Returns false always.
        /// </returns>
        public override bool Receive(ConsoleKeyInfo k)
        {
            return false;
        }

        /// <summary>
        /// Sends a string to the label.
        /// </summary>
        /// <param name="s">
        ///     The string that shall be sent.
        /// </param>
        /// <returns>
        ///     Returns false always.
        /// </returns>
        public override bool Receive(string s)
        {
            return false;
        }

        /// <summary>
        /// Raises the <see cref="TextChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///    Contains additional information for this event.
        /// </param>
        protected void OnTextChanged(EventArgs e)
        {
            if (this.TextChanged != null)
            {
                this.TextChanged(this, e);
            }
        }

        /// <summary>
        /// Called when the <see cref="Text"/> property was changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void Label_TextChanged(object sender, EventArgs e)
        {
            this.Draw(this.Rectangle);
        }
    }
}
