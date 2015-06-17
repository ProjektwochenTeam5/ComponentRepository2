using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public abstract class Message
    {
        /// <summary>
        /// The next message ID.
        /// </summary>
        private static int nextID = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        public Message()
        {
            this.MessageID = nextID++;
        }

        /// <summary>
        /// Gets the message type of the message.
        /// </summary>
        /// <value>
        ///     Contains the message type of the message.
        /// </value>
        public abstract StatusCode MessageType { get; }

        /// <summary>
        /// Gets the message's ID.
        /// </summary>
        /// <value>
        /// Contains the message's ID.
        /// </value>
        public int MessageID { get; private set; }
    }
}
