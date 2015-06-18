
namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ClientServerCommunication;

    /// <summary>
    /// Manages messages received by a client.
    /// </summary>
    public class ClientMessageManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMessageManager"/> class.
        /// </summary>
        /// <param name="managed">
        ///     The <see cref="Client"/> instance that shall be managed.
        /// </param>
        public ClientMessageManager(Client managed)
        {
            this.ReceivedMessages = new Dictionary<int, Message>(512);
            this.ManagedClient = managed;
            this.ManagedClient.ReceivedTCPMessage += this.ManagedClient_ReceivedTCPMessage;
        }

        /// <summary>
        /// Gets the received messages with their IDs.
        /// </summary>
        /// <value>
        ///     Contains the received messages with their IDs.
        /// </value>
        public Dictionary<int, Message> ReceivedMessages
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the managed client.
        /// </summary>
        /// <value>
        ///     Contains the managed client.
        /// </value>
        public Client ManagedClient
        {
            get;
            private set;
        }

        /// <summary>
        /// Called when the managed client received a message.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void ManagedClient_ReceivedTCPMessage(object sender, MessageReceivedEventArgs e)
        {
            this.ReceivedMessages.Add(e.ReceivedMessage.MessageID, e.ReceivedMessage);
        }
    }
}
