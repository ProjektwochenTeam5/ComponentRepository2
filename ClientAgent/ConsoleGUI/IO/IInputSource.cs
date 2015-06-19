// --------------------------------------------------------------
// <copyright file="IInputSource.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="IInputSource"/> interface.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI.IO
{
    using System;

    /// <summary>
    /// Provides an interface for all kinds of key inputs.
    /// </summary>
    public interface IInputSource
    {
        /// <summary>
        /// Raised when an input was read.
        /// </summary>
        event EventHandler<InputReceivedEventArgs> InputReceived;

        /// <summary>
        /// Starts reading from the input source.
        /// </summary>
        void StartReading();

        /// <summary>
        /// Stops reading from the input source.
        /// </summary>
        void StopReading();
    }
}
