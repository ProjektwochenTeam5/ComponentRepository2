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
    using Core.Network;
    using ConsoleGUI;

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
        /// 
        /// </summary>
        /// <param name="e"></param>
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

                        TransferJobResponse tjs = new TransferJobResponse();
                        tjs.Result = JobExecutor.Execute(pth, rq.InputData).ToArray();
                        tjs.BelongsToRequest = rq.JobID;
                        this.ManagedClient.SendMessage(tjs);
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

            if (!sent && m.MetadataComponents.Count > 2)
            {
                sent = true;

                /*
                var addGuid = this.StoredComponentInfos.First(c => c.FriendlyName == "Add").ComponentGuid;
                var inpGuid = this.StoredComponentInfos.First(c => c.FriendlyName == "Console Int Input").ComponentGuid;
                var outGuid = this.StoredComponentInfos.First(c => c.FriendlyName == "Console Output").ComponentGuid;

                var intaddGuid1 = Guid.NewGuid();
                var intaddGuid2 = Guid.NewGuid();
                var intinpGuid1 = Guid.NewGuid();
                var intinpGuid2 = Guid.NewGuid();
                var intinpGuid3 = Guid.NewGuid();
                var intoutGuid = Guid.NewGuid();
                
                //////////////// Testjob
                Component job = new Component();
                job.ComponentGuid = new Guid();
                job.FriendlyName = "Addieren von 3 Zahlen";
                job.InputDescriptions = new List<string>() { "zahl1", "zahl2", "zahl3" };
                job.OutputDescriptions = new List<string>() { "zahl" };
                job.InputHints = new List<string>() { typeof(int).ToString(), typeof(int).ToString(), typeof(int).ToString() };
                job.IsAtomic = false;
                job.Edges = new List<ComponentEdge>();

                var edge1 = new ComponentEdge();
                edge1.InputComponentGuid = addGuid;
                edge1.OutputComponentGuid = inpGuid;
                edge1.InternalInputComponentGuid = intaddGuid1;
                edge1.InternalOutputComponentGuid = intinpGuid1;
                edge1.InputValueID = 1;
                edge1.OutputValueID = 1;

                var edge2 = new ComponentEdge();
                edge2.InputComponentGuid = addGuid;
                edge2.OutputComponentGuid = inpGuid;
                edge2.InternalInputComponentGuid = intaddGuid1;
                edge2.InternalOutputComponentGuid = intinpGuid2;
                edge2.InputValueID = 2;
                edge2.OutputValueID = 1;

                var edge3 = new ComponentEdge();
                edge3.InputComponentGuid = addGuid;
                edge3.OutputComponentGuid = addGuid;
                edge3.InternalInputComponentGuid = intaddGuid2;
                edge3.InternalOutputComponentGuid = intaddGuid1;
                edge3.InputValueID = 1;
                edge3.OutputValueID = 1;

                var edge4 = new ComponentEdge();
                edge4.InputComponentGuid = addGuid;
                edge4.OutputComponentGuid = inpGuid;
                edge4.InternalInputComponentGuid = intaddGuid2;
                edge4.InternalOutputComponentGuid = intinpGuid3;
                edge4.InputValueID = 2;
                edge4.OutputValueID = 1;

                var edge5 = new ComponentEdge();
                edge5.InputComponentGuid = outGuid;
                edge5.OutputComponentGuid = addGuid;
                edge5.InternalInputComponentGuid = intoutGuid;
                edge5.InternalOutputComponentGuid = intaddGuid2;
                edge5.InputValueID = 1;
                edge5.OutputValueID = 1;

                List<ComponentEdge> myedges = new List<ComponentEdge>();

                myedges.Add(edge1);
                myedges.Add(edge2);
                myedges.Add(edge3);
                myedges.Add(edge4);
                myedges.Add(edge5);
                job.Edges = myedges.AsEnumerable();
                DoJobRequest rq = new DoJobRequest();
                rq.Job = job;
                this.WaitingMessages.Add(rq);
                this.ManagedClient.SendMessage(rq);
                */
            }

            this.OnReceivedLogEntry(new StringEventArgs(string.Format(
                "\nTotal elements found: {0}\n",
                m.MetadataComponents.Count)));
        }

        static bool sent = false;

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
