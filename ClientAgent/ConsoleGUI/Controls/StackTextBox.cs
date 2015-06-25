// --------------------------------------------------------------
// <copyright file="StackTextBox.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="StackTextBox"/> class.
// </summary>
// <author>
// David Eiwen
// </author>
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
    /// 
    /// </summary>
    public class StackTextBox : Control
    {
        /// <summary>
        /// Contains the value for the <see cref="Text"/> property.
        /// </summary>
        private string textProperty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputs"></param>
        public StackTextBox(ICollection<IRenderer> outputs)
            : base(outputs)
        {
            this.Lines = new List<string>(this.Rectangle.Height + 1);
            this.TextAppended += this.StackTextBox_TextAppended;
        }

        public event EventHandler TextAppended;

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get
            {
                return this.textProperty;
            }

            private set
            {
                this.textProperty = value;
                this.OnTextAppended(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<string> Lines
        {
            get;
            set;
        }

        /// <summary>
        /// Appeds a line to the text.
        /// </summary>
        /// <param name="line">
        ///     The string that shall be appended.
        /// </param>
        public void PushLine(StringEventArgs line)
        {
            if (this.Lines.Count >= this.Rectangle.Height)
            {
                this.Lines.RemoveRange(0, this.Lines.Count - this.Rectangle.Height);
            }

            this.Lines.Add(string.Format("{0}: {1}", line.TimeStamp, line.String));
            this.Text += string.Format("{0}\n", line);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Pixel[,] GetPixels()
        {
            Pixel[,] ret = new Pixel[this.Rectangle.Width, this.Rectangle.Height];
            int mid = this.Rectangle.Height / 2;

            for (int y = 0; y < this.Rectangle.Height; y++)
            {
                // get the line string
                string curentLine = this.Lines.Count > y ? this.Lines[y] : string.Empty;

                for (int x = 0; x < this.Rectangle.Width; x++)
                {
                    ret[x, y] = new Pixel(
                        this.BackgroundColor,
                        this.ForegroundColor,
                        x < curentLine.Length ? curentLine[x] : ' ');
                }
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public override bool Receive(ConsoleKeyInfo k)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public override bool Receive(string s)
        {
            this.PushLine(new StringEventArgs(string.Format("{0}\n", s)));
            return true;
        }

        /// <summary>
        /// Raises the <see cref="TextAppended"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnTextAppended(EventArgs e)
        {
            if (this.TextAppended != null)
            {
                this.TextAppended(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StackTextBox_TextAppended(object sender, EventArgs e)
        {
            this.Draw(this.Rectangle);
        }
    }
}
