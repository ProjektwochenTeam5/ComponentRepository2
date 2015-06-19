// --------------------------------------------------------------
// <copyright file="InputReceivedEventArgs.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="InputReceivedEventArgs"/> class.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI.IO
{
    using System;

    /// <summary>
    /// Provides information about a read key.
    /// </summary>
    public class InputReceivedEventArgs
    {
        /// <summary>
        /// Contains the read key.
        /// </summary>
        private ConsoleKeyInfo receivedKeyProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="k">
        ///     The key that was received.
        /// </param>
        public InputReceivedEventArgs(ConsoleKeyInfo k)
        {
            this.ReceivedKey = k;
        }

        /// <summary>
        /// Gets the received key.
        /// </summary>
        /// <value>
        ///     Contains the received key.
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown when the key shall be processed more than once.
        /// </exception>
        public ConsoleKeyInfo ReceivedKey
        {
            get
            {
                if (this.Processed)
                {
                    throw new InvalidOperationException("The key is already processed!");
                }

                return this.receivedKeyProperty;
            }

            private set
            {
                this.receivedKeyProperty = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the key was already processed.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the key was already processed.
        /// </value>
        public bool Processed
        {
            get;
            private set;
        }

        /// <summary>
        /// Processes the input.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown when the input was already processed.
        /// </exception>
        public void Process()
        {
            if (this.Processed)
            {
                throw new InvalidOperationException("The input is already processed.");
            }

            this.Processed = true;
        }
    }
}
