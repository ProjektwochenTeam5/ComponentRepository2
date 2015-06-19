// --------------------------------------------------------------
// <copyright file="TextBox.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="TextBox"/> class.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using ConsoleGUI.IO;

    /// <summary>
    /// Provides a text box.
    /// </summary>
    public class TextBox
        : Control
    {
        /// <summary>
        /// The current cursor position.
        /// </summary>
        private int cursorPos = 0;

        /// <summary>
        /// The value for the <see cref="TextBox.Text"/> property.
        /// </summary>
        private string textProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBox"/> class.
        /// </summary>
        /// <param name="renderer">
        ///     The renderer used for this text box.
        /// </param>
        public TextBox(ICollection<IRenderer> renderer)
            : base(renderer)
        {
            this.textProperty = "TextBox";
            this.Rectangle = new Rectangle(0, 0, 8, 1);
            this.BackgroundColor = ConsoleColor.DarkGray;
            this.ForegroundColor = ConsoleColor.White;
            this.CursorBackgroundColor = ConsoleColor.Gray;
            this.CursorForegroundColor = ConsoleColor.Black;
            this.TextChanged += this.TextBox_TextChanged;
        }

        /// <summary>
        /// Raised when the <see cref="TextBox.Text"/> property was changed.
        /// </summary>
        public event EventHandler TextChanged;

        /// <summary>
        /// Gets or sets the cursor's background color of this <see cref="TextBox"/> instance.
        /// </summary>
        /// <value>
        ///     Contains the cursor's background color of this <see cref="TextBox"/> instance.
        /// </value>
        public ConsoleColor CursorBackgroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cursor's foreground color of this <see cref="TextBox"/> instance.
        /// </summary>
        /// <value>
        ///     Contains the cursor's foreground color of this <see cref="TextBox"/> instance.
        /// </value>
        public ConsoleColor CursorForegroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text of the <see cref="TextBox"/> instance.
        /// </summary>
        /// <value>
        ///     Contains the text of the <see cref="TextBox"/> instance.
        /// </value>
        public string Text
        {
            get
            {
                return this.textProperty;
            }

            set
            {
                int diff = value.Length - this.textProperty.Length;
                this.textProperty = value;
                this.cursorPos = Math.Max(0, Math.Min(this.cursorPos + diff, value.Length));
                this.OnTextChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the pixels of this <see cref="TextBox"/> instance.
        /// </summary>
        /// <returns>
        ///     Returns a 2-dimensional <see cref="Server.Pixel"/> array representing the display of this <see cref="TextBox"/> instance.
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
                ConsoleColor bgc = this.BackgroundColor;
                ConsoleColor fgc = this.ForegroundColor;

                if (x - 1 == this.cursorPos)
                {
                    bgc = this.CursorBackgroundColor;
                    fgc = this.CursorForegroundColor;
                }

                if (x - 1 < this.Text.Length)
                {
                    c = this.Text[x - 1];
                }

                ret[x, mid] = new Pixel(bgc, fgc, c);
            }

            ret[0, mid] = new Pixel(this.BackgroundColor, this.ForegroundColor, ' ');
            ret[this.Rectangle.Width - 1, mid] = new Pixel(this.BackgroundColor, this.ForegroundColor, ' ');

            return ret;
        }

        /// <summary>
        /// Sends a key to this <see cref="TextBox"/> instance.
        /// </summary>
        /// <param name="k">
        ///     The key that shall be sent.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the key was accepted.
        /// </returns>
        public override bool Receive(ConsoleKeyInfo k)
        {
            if (char.IsLetterOrDigit(k.KeyChar) ||
                char.IsPunctuation(k.KeyChar) ||
                char.IsSeparator(k.KeyChar) ||
                char.IsWhiteSpace(k.KeyChar))
            {
                this.Text = this.Text.Insert(this.cursorPos, k.KeyChar.ToString());
                return true;
            }

            switch (k.Key)
            {
                case ConsoleKey.LeftArrow:
                    this.cursorPos = Math.Max(0, this.cursorPos - 1);
                    this.Draw(this.Rectangle);
                    return true;

                case ConsoleKey.RightArrow:
                    this.cursorPos = Math.Min(this.cursorPos + 1, this.Text.Length);
                    this.Draw(this.Rectangle);
                    return true;

                case ConsoleKey.Delete:
                    if (this.cursorPos < this.Text.Length)
                    {
                        int oldPos = this.cursorPos;
                        this.Text = this.Text.Remove(this.cursorPos, 1);
                        this.cursorPos = oldPos;
                        this.Draw(this.Rectangle);
                    }

                    return true;

                case ConsoleKey.Backspace:
                    if (this.cursorPos > 0 && this.cursorPos <= this.Text.Length)
                    {
                        this.Text = this.Text.Remove(this.cursorPos - 1, 1);
                        this.Draw(this.Rectangle);
                    }

                    return true;
            }

            return false;
        }

        /// <summary>
        /// Sends a string to this <see cref="TextBox"/> instance.
        /// </summary>
        /// <param name="s">
        ///     The string that shall be sent.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the string was accepted.
        /// </returns>
        public override bool Receive(string s)
        {
            return false;
        }

        /// <summary>
        /// Raises the <see cref="TextBox.TextChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnTextChanged(EventArgs e)
        {
            if (this.TextChanged != null)
            {
                this.TextChanged(this, e);
            }
        }

        /// <summary>
        /// Called when the <see cref="TextBox.Text"/> value was changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            this.Draw(this.Rectangle);
        }
    }
}
