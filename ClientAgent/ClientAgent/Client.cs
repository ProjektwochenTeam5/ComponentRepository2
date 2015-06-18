
namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using ClientServerCommunication;
    using Core.Network;

    /// <summary>
    /// Provides an abstract view on a client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The thread listening for messages from the connected server.
        /// </summary>
        private Thread listenerThread;

        /// <summary>
        /// The thread listening for a UDP server answer.
        /// </summary>
        private Thread udpListenerThread;

        /// <summary>
        /// The arguments passed to the TCP connection listener thread.
        /// </summary>
        private ClientListenerThreadArgs listenerThreadArgs;

        /// <summary>
        /// The arguments passed to the UDP server listener thread.
        /// </summary>
        private UdpClientThreadArgs udpListenerThreadArgs;

        /// <summary>
        /// The binary formatter used for serializing and de-serializing messages.
        /// </summary>
        private BinaryFormatter formatter;

        /// <summary>
        /// The list of messages waiting for an acknowledgement.
        /// </summary>
        private List<Message> waitingMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="cl">
        ///     The <see cref="TcpClient"/> instance that is used by the client.
        /// </param>
        public Client(TcpClient cl)
        {
            this.ConnectionClient = cl;
            this.waitingMessages = new List<Message>();
            this.StoredComponents = new ReadOnlyCollection<ComponentInfo>(new ComponentInfo[0]);
            this.Connected += this.Client_Connected;
            this.ReceivedTCPMessage += this.Client_ReceivedTCPMessage;
            this.formatter = new BinaryFormatter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_ReceivedTCPMessage(object sender, MessageReceivedEventArgs e)
        {
            switch (e.ReceivedMessage.MessageType)
            {
                case StatusCode.SendComponentInfos:
                    this.ReceivedSendComponentInfos(e);
                    break;

                case StatusCode.Acknowledge:
                    this.ReceivedAcknowledge(e);
                    break;

                case StatusCode.Error:
                    Console.WriteLine(((Error)e.ReceivedMessage).Message);
                    break;
            }
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

            Console.WriteLine("Received Error for {0}: {1}", received.BelongsTo, received.Message);

            Message snd = this.waitingMessages.FirstOrDefault(msg => msg.MessageID == received.BelongsTo);

            if (snd == null)
            {
                return;
            }

            this.waitingMessages.Remove(snd);
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

            Console.WriteLine("Received Acknowledge for {0}!", received.BelongsTo);

            Message snd = this.waitingMessages.FirstOrDefault(msg => msg.MessageID == received.BelongsTo);

            if (snd == null)
            {
                return;
            }

            this.waitingMessages.Remove(snd);
        }

        /// <summary>
        /// Processes a received Store Components message.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
        private void ReceivedStoreComponents(MessageReceivedEventArgs e)
        {
        }

        /// <summary>
        /// Raised when the client has successfully connected to a server.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Raised when the client has or was disconnected.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Raised when the client received a TCP message.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> ReceivedTCPMessage;

        /// <summary>
        /// Gets a value indicating whether the client is connected.
        /// </summary>
        /// <value>
        ///     Contains a value indicating whether the client is connected.
        /// </value>
        public bool IsConnected
        {
            get
            {
                return this.ConnectionClient.Connected;
            }
        }

        /// <summary>
        /// Gets the list of the stored components.
        /// </summary>
        /// <value>
        ///     Contains the list of the stored components.
        /// </value>
        public ReadOnlyCollection<ComponentInfo> StoredComponents
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the IP end point of the connected end point.
        /// </summary>
        /// <value>
        ///     Contains the IP end point of the connected end point.
        /// </value>
        public IPEndPoint ConnectedEndPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="System.Net.Sockets.TcpClient"/> instance linked to the client.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="System.Net.Sockets.TcpClient"/> instance linked to the client.
        /// </value>
        protected TcpClient ConnectionClient
        {
            get;
            private set;
        }

        /// <summary>
        /// Starts searching for a server.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown when the <see cref="Client"/> instance is already connected or does already listen for connections.
        /// </exception>
        public void StartConnectionSearch()
        {
            if ((this.udpListenerThread != null && this.udpListenerThread.IsAlive) ||
                this.IsConnected)
            {
                throw new InvalidOperationException();
            }

            this.udpListenerThread = new Thread(UdpThread);
            this.udpListenerThreadArgs = new UdpClientThreadArgs(new UdpClient(new IPEndPoint(IPAddress.Any, 1234)), this);
            this.udpListenerThread.Start(this.udpListenerThreadArgs);
        }

        /// <summary>
        /// Connects this <see cref="Client"/> instance to a server.
        /// </summary>
        /// <param name="pt">
        ///     The end point the client shall connect to.
        /// </param>
        /// <exception cref="System.Net.SocketException">
        ///     Thrown when the connection is rejected by the recipient.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown when the client is already connected to a server.
        /// </exception>
        public void ConnectTo(IPEndPoint pt)
        {
            if (this.IsConnected)
            {
                throw new InvalidOperationException();
            }

            try
            {
                this.ConnectionClient.Connect(pt);
                this.ConnectedEndPoint = pt;
                this.OnConnected(EventArgs.Empty);
            }
            catch (SocketException)
            {
                throw;
            }
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            this.SendMessage(new KeepAlive() { Terminate = true }, StatusCode.KeepAlive);
            this.ConnectionClient.Close();
        }

        /// <summary>
        /// Stops all activity of the client.
        /// </summary>
        public void StopAll()
        {
            if (this.listenerThreadArgs != null)
            {
                this.listenerThreadArgs.Stop();
            }

            if (this.udpListenerThreadArgs != null)
            {
                this.udpListenerThreadArgs.Stop();
            }
        }

        /// <summary>
        /// Sends a message to the connected host.
        /// </summary>
        /// <param name="msg">
        ///     The message that shall be sent.
        /// </param>
        /// <param name="messageType">
        ///     The type code of the sent message.
        /// </param>
        public void SendMessage(Message msg, StatusCode messageType)
        {
            MemoryStream ms = new MemoryStream();
            this.formatter.Serialize(ms, msg);

            uint length = (uint)ms.Length;
            byte b1, b100, b10000, b1000000;

            b1 = (byte)(length % 0x100);
            b100 = (byte)((length / 0x100) % 0x100);
            b10000 = (byte)((length / 0x10000) % 0x100);
            b1000000 = (byte)((length / 0x1000000) % 0x100);

            List<byte> mes = new List<byte>();
            mes.AddRange(new byte[] { 0, 0, 0, 0, b1, b100, b10000, b1000000, (byte)messageType });
            mes.AddRange(ms.ToArray());

            this.ConnectionClient.GetStream().Write(mes.ToArray(), 0, mes.Count);
        }

        /// <summary>
        /// Raises the <see cref="Connected"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnConnected(EventArgs e)
        {
            if (this.Connected != null)
            {
                this.Connected(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Disconnected"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        protected void OnDisconnected(EventArgs e)
        {
            if (this.Disconnected != null)
            {
                this.Disconnected(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ReceivedTCPMessage"/> event.
        /// </summary>
        /// <param name="e">
        ///     Contains the received message.
        /// </param>
        protected void OnReceivedTCPMessage(MessageReceivedEventArgs e)
        {
            if (this.ReceivedTCPMessage != null)
            {
                this.ReceivedTCPMessage(this, e);
            }
        }

        /// <summary>
        /// The thread that listens for server answers.
        /// </summary>
        /// <param name="data">
        ///     The arguments passed to the thread.
        /// </param>
        private static void UdpThread(object data)
        {
            UdpClientThreadArgs args = (UdpClientThreadArgs)data;
            IPEndPoint localRcv = new IPEndPoint(IPAddress.Any, 1234);
            DateTime lastSend = new DateTime(0);

            while (!args.Stopped)
            {
                if (DateTime.Now.Subtract(lastSend).TotalSeconds > 5)
                {
                    try
                    {
                        args.Client.SendDiscover(args.UdpClient);
                    }
                    catch
                    {
                        break;
                    }
                    
                    lastSend = DateTime.Now;
                }

                if (args.UdpClient.Available > 0)
                {
                    byte[] msg = args.UdpClient.Receive(ref localRcv);

                    if (msg[0] != 1 || msg[1] != 1 || msg[2] != 1 || msg[3] != 1)
                    {
                        continue;
                    }

                    int length = msg[4] + (msg[5] * 0x100) + (msg[6] * 0x10000) + (msg[7] * 0x1000000);
                    StatusCode type = (StatusCode)msg[8];

                    if (type != StatusCode.AgentConnection)
                    {
                        continue;
                    }

                    byte[] body = new byte[length];

                    for (int n = 9; n < msg.Length; n++)
                    {
                        body[n - 9] = msg[n];
                    }

                    using (MemoryStream ms = new MemoryStream(body, false))
                    {
                        AgentAccept a = (AgentAccept)args.Client.formatter.Deserialize(ms);
                        Console.WriteLine(a.ServerIP);
                        args.Stop();
                        Console.WriteLine("Trying to connect...");
                        args.Client.ConnectTo(a.ServerIP);
                        Console.WriteLine("Connected");
                    }
                }

                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// The thread that listens for the connected server.
        /// </summary>
        /// <param name="data">
        ///     The arguments passed to the thread.
        /// </param>
        private static void ListenerThread(object data)
        {
            ClientListenerThreadArgs args = (ClientListenerThreadArgs)data;
            NetworkStream str = args.Client.ConnectionClient.GetStream();
            DateTime lastKeepAlive = new DateTime(0);
            PerformanceCounter cpu = new PerformanceCounter();
            cpu.CategoryName = "Processor";
            cpu.CounterName = "% Processor Time";
            cpu.InstanceName = "_Total";

            /*StoreComponent s = new StoreComponent();
            args.Client.waitingMessages.Add(s);
            s.Component = File.ReadAllBytes("Add.dll");
            args.Client.SendMessage(s, StatusCode.StorComponent);*/

            while (!args.Stopped)
            {
                // send keep alive
                if (DateTime.Now.Subtract(lastKeepAlive).TotalSeconds >= 10)
                {
                    Console.WriteLine("Keep Alive {0}", DateTime.Now);
                    lastKeepAlive = DateTime.Now;
                    args.Client.SendMessage(new KeepAlive() { CPUWorkload = cpu.NextValue() }, 0);
                }

                while (str.DataAvailable)
                {
                    byte[] hdr = new byte[9];

                    int hlen = str.Read(hdr, 0, 9);
                    uint bodylen;
                    StatusCode messagType;

                    // go to next iteration if header lengh != 9
                    if (!ParseHeader(hdr, out bodylen, out messagType))
                    {
                        break;
                    }

                    byte[] body = new byte[bodylen];
                    int rcvbody = str.Read(body, 0, (int)bodylen);

                    using (MemoryStream ms = new MemoryStream(body))
                    {
                        Message rcv = (Message)args.Client.formatter.Deserialize(ms);
                        args.Client.OnReceivedTCPMessage(new MessageReceivedEventArgs(rcv, args.Client.ConnectedEndPoint));
                    }
                }

                Thread.Sleep(5);
            }

            args.Client.Disconnect();
        }

        /// <summary>
        /// Parses a byte array and returns whether the byte array is a valid header.
        /// </summary>
        /// <param name="header">
        ///     The byte array that shall be parsed.
        /// </param>
        /// <param name="length">
        ///     The length of the following body.
        /// </param>
        /// <param name="status">
        ///     The message status.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the specified byte array is a valid header.
        /// </returns>
        private static bool ParseHeader(byte[] header, out uint length, out StatusCode status)
        {
            length = 0;
            status = StatusCode.KeepAlive;

            if (header.Length != 9)
            {
                return false;
            }

            if (header[0] != 1 || header[1] != 1 || header[2] != 1 || header[3] != 1)
            {
                return false;
            }

            length = (uint)(header[4] + (header[5] * 0x100) + (header[6] * 0x10000) + (header[7] * 0x1000000));
            
            status = (StatusCode)header[8];
            return true;
        }

        /// <summary>
        /// Parses a byte array and returns whether the byte array is a valid UDP server message header.
        /// </summary>
        /// <param name="header">
        ///     The byte array that shall be parsed.
        /// </param>
        /// <param name="length">
        ///     The length of the following body.
        /// </param>
        /// <param name="status">
        ///     The message status.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the specified byte array is a valid UDP server message header.
        /// </returns>
        private static bool ParseUDPHeader(byte[] header, out uint length, out StatusCode status)
        {
            length = 0;
            status = StatusCode.KeepAlive;

            if (header.Length != 9)
            {
                return false;
            }

            if (header[0] != 1 || header[1] != 1 || header[2] != 1 || header[3] != 1)
            {
                return false;
            }

            length = (uint)(header[4] + (header[5] * 0x100) + (header[6] * 0x10000) + (header[7] * 0x1000000));

            status = (StatusCode)header[8];
            return true;
        }

        /// <summary>
        /// Sends a UDP broadcast message indicating that the client searches for a server.
        /// </summary>
        /// <param name="cl">
        ///     The UDP client used for the transmission.
        /// </param>
        private void SendDiscover(UdpClient cl)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1 });

            UdpBroadcast.SendBoadcast(1233, msg.ToArray());
            ////cl.Send(msg.ToArray(), msg.Count, new IPEndPoint(IPAddress.Broadcast, 1234));
        }

        /// <summary>
        /// Called when the client has connected.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void Client_Connected(object sender, EventArgs e)
        {
            this.listenerThread = new Thread(ListenerThread);
            this.listenerThreadArgs = new ClientListenerThreadArgs(this);
            this.listenerThread.Start(this.listenerThreadArgs);
        }
    }
}