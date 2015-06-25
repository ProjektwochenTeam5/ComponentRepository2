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
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ClientServerCommunication;
    using ConsoleGUI;
    using Core.Network;

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
        /// The value for the <see cref="ExecutingJobs"/> property.
        /// </summary>
        private Collection<Guid> executingJobs;

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
            this.executingJobs = new Collection<Guid>();
            this.ManagedClient = managed;
            this.ManagedClient.ReceivedTCPMessage += this.ManagedClient_ReceivedTCPMessage;
            this.ManagedClient.ReceivedLogEntry += this.ManagedClient_ReceivedLogEntry;
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
        /// Raised when the manager received a log entry.
        /// </summary>
        public event EventHandler<StringEventArgs> ReceivedLogEntry;

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
            get
            {
                return new ReadOnlyCollection<Component>(this.storedComponentInfos);
            }
        }

        /// <summary>
        /// Gets the file names of the stored components.
        /// </summary>
        /// <value>
        ///     Contains the file names of the stored components.
        /// </value>
        public Dictionary<Guid, string> StoredComponents
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of running jobs.
        /// </summary>
        /// <value>
        ///     Contians the list of running jobs.
        /// </value>
        public ReadOnlyCollection<Guid> ExecutingJobs
        {
            get { return new ReadOnlyCollection<Guid>(this.executingJobs); }
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
        /// Raises the <see cref="ReceivedTransferComponentResponseMessage"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
        protected void OnReceivedTransferComponentResponseMessage(MessageReceivedEventArgs e)
        {
            if (this.ReceivedTransferComponentResponseMessage != null)
            {
                this.ReceivedTransferComponentResponseMessage(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ReceivedTansferJobRequestMessage"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
        protected void OnReceivedTransferJobRequestMessage(MessageReceivedEventArgs e)
        {
            if (this.ReceivedTransferJobRequestMessage != null)
            {
                this.ReceivedTransferJobRequestMessage(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ReceivedLogEntry"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains the string that shall be logged.
        /// </param>
        protected void OnReceivedLogEntry(StringEventArgs e)
        {
            if (this.ReceivedLogEntry != null)
            {
                this.ReceivedLogEntry(this, e);
            }
        }

        /// <summary>
        /// Called when the client received a log entry.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void ManagedClient_ReceivedLogEntry(object sender, StringEventArgs e)
        {
            this.OnReceivedLogEntry(e);
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
            this.OnReceivedMessage(e);
            switch (e.ReceivedMessage.MessageType)
            {
                case StatusCode.Error:
                    this.ReceivedEror(e);
                    this.OnReceivedErrorMessage(e);
                    return;

                case StatusCode.Acknowledge:
                    this.ReceivedAcknowledge(e);
                    this.OnReceivedAcknowledgeMessage(e);
                    return;

                case StatusCode.SendComponentInfos:
                    this.ReceivedSendComponentInfos(e);
                    this.OnReceivedSendComponentInfo(e);
                    return;

                case StatusCode.TransferComponent:
                    this.ReceivedTransferComponentResponse(e);
                    this.OnReceivedTransferComponentResponseMessage(e);
                    return;

                case StatusCode.TransferJob:
                    this.ReceivedTransferJobRequest(e);
                    this.OnReceivedTransferJobRequestMessage(e);
                    break;
            }

            this.WaitingMessages.Add(e.ReceivedMessage);
        }

        /// <summary>
        /// Called when a transfer job request was received.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void ReceivedTransferJobRequest(MessageReceivedEventArgs e)
        {
            TransferJobRequest rq = e.ReceivedMessage as TransferJobRequest;

            Thread t = new Thread(delegate()
                { 
                    while (true)
                    {
                        string cmp = this.StoredComponents.FirstOrDefault(c => c.Key == rq.ComponentGuid).Value;

                        if (cmp == default(string) || string.IsNullOrEmpty(cmp))
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

                                while (this.StoredComponents.ToArray().FirstOrDefault(c => c.Key == rq.ComponentGuid).Value == null)
                                {
                                    Thread.Sleep(5);
                                }

                                cont = true;
                                resp = args.ReceivedMessage as TransferComponentResponse;
                            };

                            this.ReceivedTransferComponentResponseMessage += d;
                            this.WaitingMessages.Add(tcr);
                            this.ManagedClient.SendMessage(tcr);

                            while (!cont)
                            {
                                Thread.Sleep(10);
                            }

                            this.ReceivedTransferComponentResponseMessage -= d;
                            continue;
                        }

                        string pth = this.StoredComponents.FirstOrDefault(k => k.Value == cmp).Value;

                        JobExecutor.GetAssemblies();
                        Assembly a = JobExecutor.Data.FirstOrDefault(k => k.Key == pth).Value;

                        if (a == null)
                        {
                            return;
                        }

                        this.executingJobs.Add(this.StoredComponents.FirstOrDefault(k => k.Value == cmp).Key);
                        TransferJobResponse tjs = new TransferJobResponse();
                        tjs.Result = JobExecutor.Execute(pth, rq.InputData).ToArray();
                        tjs.BelongsToRequest = rq.JobID;
                        this.ManagedClient.SendMessage(tjs);
                        this.executingJobs.Remove(this.StoredComponents.FirstOrDefault(k => k.Value == cmp).Key);
                        return;
                    }
                });

            t.Start();
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

            TransferComponentRequest rq = this.WaitingMessages.ToArray().FirstOrDefault(msg => msg.MessageID == r.BelongsTo) as TransferComponentRequest;

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

            string filename = string.Format("temp\\{0}.dll", c.FriendlyName);
            JobExecutor.StoreComponent(r.Component, filename);
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
            this.OnReceivedLogEntry(new StringEventArgs(new[]
            { 
                string.Format(
                "Error from {0}: {1}",
                snd.MessageID,
                received.Message)
            }));

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

            this.OnReceivedLogEntry(new StringEventArgs(new[]
            {
                string.Format(
                    "{0}|{1}",
                    "Friendly Name".PadRight(40),
                    "Guid")
            }));

            List<string> strs = new List<string>(m.MetadataComponents.Count);

            foreach (Component c in m.MetadataComponents)
            {
                try
                {
                    strs.Add(string.Format(
                        "{0}|{1}",
                        c.FriendlyName.PadRight(40),
                        c.ComponentGuid));
                }
                catch
                {
                }
            }

            strs.Add(string.Format(
                "Total elements found: {0}",
                m.MetadataComponents.Count));

            this.OnReceivedLogEntry(new StringEventArgs(strs.ToArray()));
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

            this.OnReceivedLogEntry(new StringEventArgs(new[]
            {
                string.Format(
                    "Acknowledge {0}!",
                    received.BelongsTo)
            }));

            Message snd = this.WaitingMessages.FirstOrDefault(msg => msg.MessageID == received.BelongsTo);

            if (snd == null)
            {
                return;
            }

            this.WaitingMessages.Remove(snd);
        }
    }
}
