
namespace ClientAgent
{
    /// <summary>
    /// Provides a way to communicate with a thread.
    /// </summary>
    public class ThreadArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadArgs"/> class.
        /// </summary>
        public ThreadArgs()
        {
            this.Stopped = false;
        }

        /// <summary>
        /// Gets a value indicating whether the thread was stopped.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the thread was stopped.
        /// </value>
        public bool Stopped
        {
            get;
            private set;
        }

        /// <summary>
        /// Stops the thread.
        /// </summary>
        public void Stop()
        {
            this.Stopped = true;
        }
    }
}
