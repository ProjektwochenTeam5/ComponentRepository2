
namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ClientServerCommunication;
    using Core.Network;

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
            this.WaitingMessages = new List<Message>(512);
            this.ManagedClient = managed;
            this.ManagedClient.ReceivedTCPMessage += this.ManagedClient_ReceivedTCPMessage;
        }

        /// <summary>
        /// Gets the received messages with their IDs.
        /// </summary>
        /// <value>
        ///     Contains the received messages with their IDs.
        /// </value>
        public List<Message> WaitingMessages
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
            switch (e.ReceivedMessage.MessageType)
            {
                case StatusCode.Error:
                    this.ReceivedEror(e);
                    break;

                case StatusCode.Acknowledge:
                    this.ReceivedAcknowledge(e);
                    break;

                case StatusCode.SendComponentInfos:
                    this.ReceivedSendComponentInfos(e);
                    break;
            }

            this.WaitingMessages.Add(e.ReceivedMessage);
        }

        /// <summary>
        /// Processes a received error.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
        private void ReceivedEror(MessageReceivedEventArgs e)
        {
            Error received = e.ReceivedMessage as Error;
            if (received == null)
            {
                return;
            }

            Message snd = this.WaitingMessages.FirstOrDefault(msg => msg.MessageID == received.BelongsTo);

            if (snd == null)
            {
                return;
            }

            this.WaitingMessages.Remove(snd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void ReceivedSendComponentInfos(MessageReceivedEventArgs e)
        {
            SendComponentInfos m = (SendComponentInfos)e.ReceivedMessage;
            foreach (Component c in m.MetadataComponents)
            {
                Console.WriteLine(c.FriendlyName);
            }
        }

        /// <summary>
        /// Processes a received acknowledge message.
        /// </summary>
        /// <param name="e"></param>
        private void ReceivedAcknowledge(MessageReceivedEventArgs e)
        {
            Acknowledge received = e.ReceivedMessage as Acknowledge;
            if (received == null)
            {
                return;
            }

            Message snd = this.WaitingMessages.FirstOrDefault(msg => msg.MessageID == received.BelongsTo);

            if (snd == null)
            {
                return;
            }

            this.WaitingMessages.Remove(snd);
        }
    }
}
