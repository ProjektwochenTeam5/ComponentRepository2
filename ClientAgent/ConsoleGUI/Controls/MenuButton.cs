// --------------------------------------------------------------
// <copyright file="MenuButton.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="MenuButton"/> class.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI.Controls
{
    using System;
    using ConsoleGUI.IO;

    /// <summary>
    /// Provides information about a menu button.
    /// </summary>
    public class MenuButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuButton"/> class.
        /// </summary>
        public MenuButton()
        {
            this.Text = "MenuButton";
            this.Enabled = true;
            this.Visible = true;
        }

        /// <summary>
        /// Raised when the button key is pressed.
        /// </summary>
        public event EventHandler ButtonKeyPressed;

        /// <summary>
        /// Gets or sets a value indicating whether the button is enabled.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the button is enabled.
        /// </value>
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the button is visible.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the button is visible.
        /// </value>
        public bool Visible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text of the button.
        /// </summary>
        /// <value>
        ///     Contains the text of the button.
        /// </value>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the linked key.
        /// </summary>
        /// <value>
        ///     Contains the linked key.
        /// </value>
        public ConsoleKey LinkedKey
        {
            get;
            set;
        }

        /// <summary>
        /// Presses a key on the button.
        /// </summary>
        /// <param name="e">
        ///     Some information about the pressed key.
        /// </param>
        public void PressKey(InputReceivedEventArgs e)
        {
            if (!this.Enabled)
            {
                return;
            }

            try
            {
                if (e.ReceivedKey.Key == this.LinkedKey)
                {
                    if (this.ButtonKeyPressed != null)
                    {
                        this.ButtonKeyPressed(this, EventArgs.Empty);
                    }

                    e.Process();
                }
            }
            catch (InvalidOperationException) 
            {
                throw;
            }
        }
    }
}
