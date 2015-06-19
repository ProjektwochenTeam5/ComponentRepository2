
namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ClientServerCommunication;
    using Core.Network;
    using System.IO;

    /// <summary>
    /// Manages messages received by a client.
    /// </summary>
    public class ClientMessageManager
    {
        /// <summary>
        /// The value for the <see cref="StoredComponentInfos"/> property.
        /// </summary>
        private Collection<Component> storedComponentInfos;

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
        /// Raised when the manager received a message.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> ReceivedMessage;

        public event EventHandler<MessageReceivedEventArgs> ReceivedErrorMessage;

        public event EventHandler<MessageReceivedEventArgs> ReceivedAcknowledgeMessage;

        public event EventHandler<MessageReceivedEventArgs> ReceivedSendComponentInfoMessage;

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
        /// Gets the stored components.
        /// </summary>
        /// <value>
        ///     Contains the stored components.
        /// </value>
        public ReadOnlyCollection<Component> StoredComponentInfos
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
        /// Raises the <see cref="ReceivedMessage"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
        protected void OnReceivedMessage(MessageReceivedEventArgs e)
        {
            if (this.ReceivedMessage != null)
            {
                this.ReceivedMessage(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ReceivedAcknowledgeMessage"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
        protected void OnReceivedAcknowledgeMessage(MessageReceivedEventArgs e)
        {
            if (this.ReceivedAcknowledgeMessage != null)
            {
                this.ReceivedAcknowledgeMessage(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ReceivedErrorMessage"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
        protected void OnReceivedErrorMessage(MessageReceivedEventArgs e)
        {
            if (this.ReceivedErrorMessage != null)
            {
                this.ReceivedErrorMessage(this, e);
            }
        }

        protected void OnReceivedSendComponentInfo(MessageReceivedEventArgs e)
        {
            if (this.ReceivedSendComponentInfoMessage != null)
            {
                this.ReceivedSendComponentInfoMessage(this, e);
            }
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

                case StatusCode.TransferComponent:
                    this.ReceivedTransferComponentResponse(e);
                    break;
            }

            this.WaitingMessages.Add(e.ReceivedMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void ReceivedTransferComponentResponse(MessageReceivedEventArgs e)
        {
            TransferComponentResponse r = e.ReceivedMessage as TransferComponentResponse;
            if (r == null)
            {
                return;
            }

            TransferComponentRequest rq =
                this.WaitingMessages.FirstOrDefault(msg => msg.MessageID == r.BelongsTo) as TransferComponentRequest;

            if (rq == null)
            {
                return;
            }

            this.WaitingMessages.Remove(rq);

            Component c =
                this.storedComponentInfos.FirstOrDefault(comp => comp.ComponentGuid == rq.ComponentID);

            if (c == null)
            {
                return;
            }

            File.WriteAllBytes(c.FriendlyName + ".dll", r.Component);
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
            Console.WriteLine("Error from {0}: {1}", snd.MessageID, received.Message);

            if (snd == null)
            {
                return;
            }

            this.WaitingMessages.Remove(snd);
        }

        /// <summary>
        /// Processes a received send component info message.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
        private void ReceivedSendComponentInfos(MessageReceivedEventArgs e)
        {
            SendComponentInfos m = e.ReceivedMessage as SendComponentInfos;
            if (m == null)
            {
                return;
            }

            this.storedComponentInfos = new Collection<Component>(m.MetadataComponents.ToList());

            foreach (Component c in m.MetadataComponents)
            {
                Console.WriteLine(c.FriendlyName);
            }

            Component cm = m.MetadataComponents.First();

            TransferComponentRequest rq = new TransferComponentRequest();
            rq.ComponentID = cm.ComponentGuid;
            this.WaitingMessages.Add(rq);
            this.ManagedClient.SendMessage(rq);
        }

        /// <summary>
        /// Processes a received acknowledge message.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
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
