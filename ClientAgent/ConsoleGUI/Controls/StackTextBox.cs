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
        /// Appeds a line to the text.
        /// </summary>
        /// <param name="line">
        ///     The string that shall be appended.
        /// </param>
        public void PushLine(string line)
        {
            this.Text += string.Format("{0}\n", line);
        }

        public override Pixel[,] GetPixels()
        {
            throw new NotImplementedException();
        }

        public override bool Receive(ConsoleKeyInfo k)
        {
            throw new NotImplementedException();
        }

        public override bool Receive(string s)
        {
            this.Text += string.Format("{0}\n", s);
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
