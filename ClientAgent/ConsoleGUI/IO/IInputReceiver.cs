// --------------------------------------------------------------
// <copyright file="IInputReceiver.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="IInputReceiver"/> interface.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI.IO
{
    using System;

    /// <summary>
    /// Provides an interface for objects that can receive input.
    /// </summary>
    public interface IInputReceiver
    {
        /// <summary>
        /// Sends a key to a <see cref="IInputReceiver"/> instance.
        /// </summary>
        /// <param name="k">
        ///     The key that shall be sent.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the input was accepted.
        /// </returns>
        bool Receive(ConsoleKeyInfo k);

        /// <summary>
        /// Sends a string to a <see cref="IInputReceiver"/> instance.
        /// </summary>
        /// <param name="s">
        ///     The string that shall be sent.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the input was accepted.
        /// </returns>
        bool Receive(string s);
    }
}
