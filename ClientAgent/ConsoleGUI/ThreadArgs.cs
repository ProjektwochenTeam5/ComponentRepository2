// --------------------------------------------------------------
// <copyright file="ThreadArgs.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="ThreadArgs"/> class.
// </summary>
// --------------------------------------------------------------

namespace ConsoleGUI
{
    /// <summary>
    /// Provides a base class for all thread argument classes. This class is abstract.
    /// </summary>
    public abstract class ThreadArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadArgs"/> class.
        /// </summary>
        protected ThreadArgs()
        {
            this.Stopped = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the underlying thread shall be stopped.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the underlying thread shall be stopped.
        /// </value>
        public bool Stopped
        {
            get;
            set;
        }
    }
}
