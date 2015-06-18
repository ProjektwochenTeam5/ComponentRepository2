
namespace ClientAgent
{
    using System;
    using System.Net;
    using ClientServerCommunication;

    /// <summary>
    /// Stores information about a received message.
    /// </summary>
    public class MessageReceivedEventArgs
        : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="rcv">
        ///     The received message.
        /// </param>
        /// <param name="from">
        ///     The end point from where the message was received.
        /// </param>
        public MessageReceivedEventArgs(Message rcv, IPEndPoint from)
            : base()
        {
            this.ReceivedMessage = rcv;
            this.From = from;
        }

        /// <summary>
        /// Gets the received message.
        /// </summary>
        /// <value>
        ///     Contains the received message.
        /// </value>
        public Message ReceivedMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the end point where the message was sent from.
        /// </summary>
        /// <value>
        ///     Contains the end point where the message was sent from.
        /// </value>
        public IPEndPoint From
        {
            get;
            private set;
        }
    }
}
