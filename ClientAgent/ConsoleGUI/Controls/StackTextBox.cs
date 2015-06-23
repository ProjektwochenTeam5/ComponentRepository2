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
        /// 
        /// </summary>
        /// <param name="outputs"></param>
        public StackTextBox(ICollection<IRenderer> outputs)
            : base(outputs)
        {

        }

        public string Text
        {
            get;
            private set;
        }

        public void PushLine()
        {

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
    }
}
