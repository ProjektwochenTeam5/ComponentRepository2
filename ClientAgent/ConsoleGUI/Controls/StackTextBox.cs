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

        }

        public string Text
        {
            get
            {
                return this.textProperty;
            }

            private set
            {
                this.textProperty = value;
                this.OnTextChanged(EventArgs.Empty);
            }
        }

        public void PushLine(string line)
        {
            this.Text += line;
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        protected void OnTextChanged(EventArgs e)
        {

        }
    }
}
