// --------------------------------------------------------------
// <copyright file="ConsoleInput.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="ConsoleInput"/> class.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Provides an interface for console input.
    /// </summary>
    public class ConsoleInput : IInputSource
    {
        /// <summary>
        /// The locker variable for multi-thread access.
        /// </summary>
        private static readonly object ReadLock = new object();

        /// <summary>
        /// The thread that reads from the console input.
        /// </summary>
        private Thread readerThread;

        /// <summary>
        /// The arguments for the reader thread.
        /// </summary>
        private InputThreadArgs readerArgs;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleInput"/> class.
        /// </summary>
        public ConsoleInput()
        {
        }

        /// <summary>
        /// Raised when an input was read from the console input.
        /// </summary>
        public event EventHandler<InputReceivedEventArgs> InputReceived;

        /// <summary>
        /// Starts reading from the console.
        /// </summary>
        public void StartReading()
        {
            if (this.readerThread != null && this.readerThread.IsAlive)
            {
                throw new InvalidOperationException("Reader is already running.");
            }

            this.readerArgs = new InputThreadArgs();
            this.readerThread = new Thread(this.Reader);
            this.readerThread.Start(this.readerArgs);
        }

        /// <summary>
        /// Stops reading from the console.
        /// </summary>
        public void StopReading()
        {
            if (this.readerThread == null || (this.readerThread != null && !this.readerThread.IsAlive))
            {
                throw new InvalidOperationException("Reader is already stopped.");
            }

            this.readerArgs.Stop();
        }

        /// <summary>
        /// Raises the <see cref="InputReceived"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnInputReceived(InputReceivedEventArgs e)
        {
            if (this.InputReceived != null)
            {
                this.InputReceived(this, e);
            }
        }

        /// <summary>
        /// The thread that reads from the console.
        /// </summary>
        /// <param name="data">
        ///     The arguments passed to the thread.
        /// </param>
        private void Reader(object data)
        {
            InputThreadArgs args = (InputThreadArgs)data;

            lock (ReadLock)
            {
                while (!args.Stopped)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo k = Console.ReadKey(true);
                        this.OnInputReceived(new InputReceivedEventArgs(k));
                    }

                    Thread.Sleep(5);
                }
            }
        }
    }
}
