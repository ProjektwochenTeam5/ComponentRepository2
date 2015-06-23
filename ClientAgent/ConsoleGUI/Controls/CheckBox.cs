// --------------------------------------------------------------
// <copyright file="CheckBox.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="CheckBox"/> class.
// </summary>
// <author>
// David Eiwen
// </author>
// --------------------------------------------------------------

namespace ConsoleGUI.Controls
{
    using System;
    using System.Collections.Generic;
    using ConsoleGUI.IO;

    /// <summary>
    /// Provides a checkbox control.
    /// </summary>
    public class CheckBox
        : Control
    {
        /// <summary>
        /// The value for the <see cref="Checked"/> property.
        /// </summary>
        private bool checkedProperty;

        /// <summary>
        /// The value for the <see cref="Text"/> property.
        /// </summary>
        private string textProperty;

        /// <summary>
        /// The value for the <see cref="CheckedColor"/> property.
        /// </summary>
        private ConsoleColor checkedColorProperty;

        /// <summary>
        /// The value for the <see cref="UncheckedColor"/> property.
        /// </summary>
        private ConsoleColor uncheckedColorProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBox"/> class.
        /// </summary>
        /// <param name="output">
        ///     The outputs the checkbox is assigned to.
        /// </param>
        public CheckBox(ICollection<IRenderer> output)
            : base(output)
        {
        }

        /// <summary>
        /// Raised when the <see cref="Checked"/> property was changed.
        /// </summary>
        public event EventHandler CheckedChanged;

        /// <summary>
        /// Raised when the <see cref="Text"/> property was changed.
        /// </summary>
        public event EventHandler TextChanged;

        /// <summary>
        /// Raised when the <see cref="CheckedColor"/> property was changed.
        /// </summary>
        public event EventHandler CheckedColorChanged;

        /// <summary>
        /// Raised when the <see cref="UncheckedColor"/> property was changed.
        /// </summary>
        public event EventHandler UncheckedColorChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the check box is checked.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the check box is checked.
        /// </value>
        public bool Checked
        {
            get
            {
                return this.checkedProperty;
            }

            set
            {
                this.checkedProperty = value;
                this.OnCheckedChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the color of the <see cref="ConsoleGUI.Controls.CheckBox"/> instance
        /// when it is checked.
        /// </summary>
        /// <value>
        ///     Contains the color of the <see cref="ConsoleGUI.Controls.CheckBox"/> instance
        ///     when it is checked.
        /// </value>
        public ConsoleColor CheckedColor
        {
            get
            {
                return this.checkedColorProperty;
            }

            set
            {
                this.checkedColorProperty = value;
                this.OnCheckedColorChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the color of the <see cref="ConsoleGUI.Controls.CheckBox"/> instance
        /// when it is not checked.
        /// </summary>
        /// <value>
        ///     Contains the color of the <see cref="ConsoleGUI.Controls.CheckBox"/> instance
        ///     when it is not checked.
        /// </value>
        public ConsoleColor UncheckedColor
        {
            get
            {
                return this.checkedColorProperty;
            }

            set
            {
                this.uncheckedColorProperty = value;
                this.OnUncheckedColorChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the text of the control.
        /// </summary>
        /// <value>
        ///     Contains the text of the control.
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
        /// Gets the pixels of the <see cref="ConsoleGUI.Controls.CheckBox"/>.
        /// </summary>
        /// <returns>
        ///     Returns a 2-dimensional <see cref="ConsoleGUI.Pixel"/> array describing how the check box shall be displayed.
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

            ConsoleColor chk = this.Checked ? this.CheckedColor : this.UncheckedColor;
            ret[0, mid] = new Pixel(chk, chk, ' ');

            // line with text
            for (int x = 2; x < this.Rectangle.Width - 1; x++)
            {
                char c = ' ';
                if (x - 1 < this.Text.Length)
                {
                    c = this.Text[x - 1];
                }

                ret[x, mid] = new Pixel(this.BackgroundColor, this.ForegroundColor, c);
            }

            ret[1, mid] = new Pixel(this.BackgroundColor, this.ForegroundColor, ' ');
            ret[this.Rectangle.Width - 1, mid] = new Pixel(this.BackgroundColor, this.ForegroundColor, ' ');

            return ret;
        }

        /// <summary>
        /// Sends a key to this <see cref="ConsoleGUI.Controls.CheckBox"/> instance.
        /// </summary>
        /// <param name="k">
        ///     The key that shall be sent.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether this <see cref="ConsoleGUI.Controls.CheckBox"/>
        ///     instance accepted the key.
        /// </returns>
        public override bool Receive(ConsoleKeyInfo k)
        {
            switch (k.Key)
            {
                case ConsoleKey.Spacebar:
                    this.Checked = !this.Checked;
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Sends a string to this <see cref="ConsoleGUI.Controls.CheckBox"/> instance.
        /// </summary>
        /// <param name="s">
        ///     The string that shall be sent.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether this <see cref="ConsoleGUI.Controls.CheckBox"/>
        ///     instance accepted the string.
        /// </returns>
        public override bool Receive(string s)
        {
            return false;
        }

        /// <summary>
        /// Raises the <see cref="CheckedChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnCheckedChanged(EventArgs e)
        {
            if (this.CheckedChanged != null)
            {
                this.CheckedChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="TextChanged"/> event.
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
        /// Raises the <see cref="CheckedColorChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnCheckedColorChanged(EventArgs e)
        {
            if (this.CheckedColorChanged != null)
            {
                this.CheckedColorChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="UncheckedColorChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnUncheckedColorChanged(EventArgs e)
        {
            if (this.UncheckedColorChanged != null)
            {
                this.UncheckedColorChanged(this, e);
            }
        }
    }
}
