
namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using ClientServerCommunication;

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
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="cl">
        ///     The <see cref="TcpClient"/> instance that is used by the client.
        /// </param>
        public Client(TcpClient cl)
        {
            this.ConnectionClient = cl;
            this.Connected += this.Client_Connected;
        }

        /// <summary>
        /// Raised when the client has successfully connected to a server.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Gets the <see cref="System.Net.Sockets.TcpClient"/> instance linked to the client.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="System.Net.Sockets.TcpClient"/> instance linked to the client.
        /// </value>
        public TcpClient ConnectionClient
        {
            get;
            private set;
        }

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
            }
            catch (SocketException)
            {
                throw;
            }

            this.OnConnected(EventArgs.Empty);
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
                    args.Client.SendDiscover(args.UdpClient);
                    lastSend = DateTime.Now;
                }

                if (args.UdpClient.Available > 0)
                {
                    byte[] msg = args.UdpClient.Receive(ref localRcv);

                    if (!(msg[0] == 1 && msg[1] == 1 && msg[2] == 1 && msg[3] == 1))
                    {
                        continue;
                    }

                    int length = msg[4] + (msg[5] * 0x100) + (msg[6] * 0x10000) + (msg[7] * 0x1000000);
                    byte type = msg[8];

                    if (type != 1)
                    {
                        Console.WriteLine("Wrong message received. Expected '1'.");
                        continue;
                    }

                    byte[] body = new byte[length];

                    for (int n = 9; n < msg.Length; n++)
                    {
                        body[n - 9] = msg[n];
                    }
                    
                    MemoryStream ms = new MemoryStream(body, false);
                    BinaryFormatter f = new BinaryFormatter();
                    
                    AgentAccept a = (AgentAccept)f.Deserialize(ms);
                    
                    Console.WriteLine(a.ServerIP);
                    args.Stop();
                    Console.WriteLine("Trying to connect...");
                    args.Client.ConnectTo(a.ServerIP);
                    Console.WriteLine("Connected");
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
            DateTime lastKeepAlive = DateTime.Now;

            while (!args.Stopped)
            {
                if (lastKeepAlive.Subtract(DateTime.Now).TotalSeconds >= 10)
                {
                    // keep alive
                    lastKeepAlive = DateTime.Now;
                    args.Client.SendMessage(new KeepAlive(), 0);
                }

                Thread.Sleep(5);
            }

            args.Client.Disconnect();
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

            cl.Send(msg.ToArray(), msg.Count, new IPEndPoint(IPAddress.Broadcast, 1234));
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
        }
    }
}