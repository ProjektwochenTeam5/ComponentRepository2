// --------------------------------------------------------------
// <copyright file="ClientMessageManager.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="ClientMessageManager"/> class.
// </summary>
// <author>
// David Eiwen
// </author>
// --------------------------------------------------------------

namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ClientServerCommunication;
    using Core.Network;
    using System.Threading;
    using System.Reflection;

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
            this.StoredComponents = new Dictionary<Guid, string>();
            this.ManagedClient = managed;
            this.ManagedClient.ReceivedTCPMessage += this.ManagedClient_ReceivedTCPMessage;
        }

        /// <summary>
        /// Raised when the manager received a message.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> ReceivedMessage;

        /// <summary>
        /// Raised when the manager received an error.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> ReceivedErrorMessage;

        /// <summary>
        /// Raised when the manager received an acknowledge.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> ReceivedAcknowledgeMessage;

        /// <summary>
        /// Raised when the manager received a send component info.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> ReceivedSendComponentInfoMessage;

        /// <summary>
        /// Raised when the manager received a transfer component response.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> ReceivedTransferComponentResponseMessage;

        /// <summary>
        /// Raised when the manager received a transfer job request.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> ReceivedTransferJobRequestMessage;

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
        /// 
        /// </summary>
        public Dictionary<Guid, string> StoredComponents
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

        /// <summary>
        /// Raises the <see cref="ReceivedSendComponentInfoMessage"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
        protected void OnReceivedSendComponentInfo(MessageReceivedEventArgs e)
        {
            if (this.ReceivedSendComponentInfoMessage != null)
            {
                this.ReceivedSendComponentInfoMessage(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">
        ///     
        /// </param>
        protected void OnReceivedTransferComponentResponseMessage(MessageReceivedEventArgs e)
        {
            if (this.ReceivedTransferComponentResponseMessage != null)
            {
                this.ReceivedTransferComponentResponseMessage(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">
        ///     
        /// </param>
        protected void OnReceivedTransferJobRequestMessage(MessageReceivedEventArgs e)
        {
            if (this.ReceivedTransferJobRequestMessage != null)
            {
                this.ReceivedTransferJobRequestMessage(this, e);
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
                    this.OnReceivedErrorMessage(e);
                    this.ReceivedEror(e);
                    break;

                case StatusCode.Acknowledge:
                    this.OnReceivedAcknowledgeMessage(e);
                    this.ReceivedAcknowledge(e);
                    break;

                case StatusCode.SendComponentInfos:
                    this.OnReceivedSendComponentInfo(e);
                    this.ReceivedSendComponentInfos(e);
                    break;

                case StatusCode.TransferComponent:
                    this.OnReceivedTransferComponentResponseMessage(e);
                    this.ReceivedTransferComponentResponse(e);
                    break;

                case StatusCode.TransferJob:
                    this.OnReceivedTransferJobRequestMessage(e);
                    this.ReceivedTransferJobRequest(e);
                    break;
            }

            this.WaitingMessages.Add(e.ReceivedMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void ReceivedTransferJobRequest(MessageReceivedEventArgs e)
        {
            TransferJobRequest rq = e.ReceivedMessage as TransferJobRequest;

            while (true)
            {
                Component cmp = this.storedComponentInfos.FirstOrDefault(c => c.ComponentGuid == rq.ComponentGuid);

                if (cmp == null)
                {
                    TransferComponentRequest tcr = new TransferComponentRequest();
                    tcr.ComponentID = rq.ComponentGuid;

                    TransferComponentResponse resp = null;
                    bool cont = false;

                    // event handler receiving a transfer component response
                    EventHandler<MessageReceivedEventArgs> d = delegate(object sender, MessageReceivedEventArgs args)
                    {
                        if (args.ReceivedMessage.MessageType != StatusCode.TransferComponent)
                        {
                            return;
                        }

                        cont = true;
                        resp = args.ReceivedMessage as TransferComponentResponse;
                    };

                    this.ReceivedTransferComponentResponseMessage += d;
                    this.ManagedClient.SendMessage(tcr);

                    while (!cont)
                    {
                        Thread.Sleep(10);
                    }

                    this.ReceivedTransferComponentResponseMessage -= d;
                    continue;
                }

                string pth = this.StoredComponents.FirstOrDefault(k => k.Key == cmp.ComponentGuid).Value;

                JobExecutor.GetAssemblies();
                Assembly a = JobExecutor.Data.FirstOrDefault(k => k.Key == pth).Value;

                if (a == null)
                {
                    return;
                }

                TransferJobResponse tjs = new TransferJobResponse();
                tjs.Result = JobExecutor.Execute(a, rq.InputData).ToArray();
                tjs.BelongsToRequest = rq.ComponentGuid;
                this.ManagedClient.SendMessage(tjs);
                return;
            }
        }

        /// <summary>
        /// Processes a received Transfer Component Response message.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
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

            File.WriteAllBytes("temp\\" + c.FriendlyName + ".dll", r.Component);
            this.StoredComponents.Add(c.ComponentGuid, c.FriendlyName + ".dll");
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

            Console.WriteLine("{0}|{1}", "Friendly Name".PadRight(40), "Guid");

            foreach (Component c in m.MetadataComponents)
            {
                try
                {
                    Console.WriteLine("{0}|{1}", c.FriendlyName.PadRight(40), c.ComponentGuid);
                }
                catch
                {
                }
            }

            if (m.MetadataComponents.Count > 0)
            {
                DoJobRequest rq = new DoJobRequest();
                rq.Job = m.MetadataComponents.First();
                this.WaitingMessages.Add(rq);
                this.ManagedClient.SendMessage(rq);
            }

            Console.WriteLine("\nTotal elements found: {0}\n", m.MetadataComponents.Count);
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

            Console.WriteLine("Acknowledge {0}!", received.BelongsTo);
            Message snd = this.WaitingMessages.FirstOrDefault(msg => msg.MessageID == received.BelongsTo);

            if (snd == null)
            {
                return;
            }

            this.WaitingMessages.Remove(snd);
        }
    }
}
