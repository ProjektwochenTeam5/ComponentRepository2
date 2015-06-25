
namespace ConsoleGUI
{
    using System;

    /// <summary>
    /// Provides arguments for a string sending event.
    /// </summary>
    public class StringEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringEventArgs"/> class.
        /// </summary>
        /// <param name="str">
        ///     The strings that shall be received by the event handlers.
        /// </param>
        public StringEventArgs(string[] str) : base()
        {
            this.String = str;
            this.TimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Gets the string that shall be received by the event handlers.
        /// </summary>
        /// <value>
        ///     Contains the string that shall be received by the event handlers.
        /// </value>
        public string[] String
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the date and time of the logging event.
        /// </summary>
        /// <value>
        ///     Contains the date and time of the logging event.
        /// </value>
        public DateTime TimeStamp
        {
            get;
            private set;
        }
    }
}
