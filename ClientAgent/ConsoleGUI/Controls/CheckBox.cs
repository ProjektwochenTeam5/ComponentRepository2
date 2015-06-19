using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGUI.Controls
{
    using ConsoleGUI.IO;
    using ConsoleGUI.Controls;

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
        /// Initializes a new instance of the <see cref="CheckBox"/> class.
        /// </summary>
        /// <param name="output">
        ///     
        /// </param>
        /// <param name="input">
        ///     
        /// </param>
        public CheckBox(ICollection<IRenderer> output, ICollection<IInputSource> input)
            : base(output, input)
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

        /// <summary>
        /// Raises the <see cref="CheckedChanged"/> event.
        /// </summary>
        /// <param name="e">
        ///     
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
        ///     
        /// </param>
        protected void OnTextChanged(EventArgs e)
        {
            if (this.TextChanged != null)
            {
                this.TextChanged(this, e);
            }
        }
    }
}
