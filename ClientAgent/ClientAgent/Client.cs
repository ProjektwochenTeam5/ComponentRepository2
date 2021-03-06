﻿// --------------------------------------------------------------
// <copyright file="Client.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="Client"/> class.
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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using ClientServerCommunication;
    using ConsoleGUI;
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
            this.formatter = new BinaryFormatter();
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
        /// Raised when the client received a log entry.
        /// </summary>
        public event EventHandler<StringEventArgs> ReceivedLogEntry;

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
        public static bool ParseHeader(byte[] header, out uint length, out StatusCode status)
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
        public static bool ParseUDPHeader(byte[] header, out uint length, out StatusCode status)
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
        /// Serializes a message to a client message.
        /// </summary>
        /// <param name="m">
        ///     The message that shall be serialized.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="byte"/> array containing the serialized message and the message header.
        /// </returns>
        public static byte[] SerializeMessage(Message m)
        {
            byte[] ret = null;
            BinaryFormatter f = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                f.Serialize(ms, m);

                long length = ms.Length;

                byte b1, b100, b10000, b1000000;

                b1 = (byte)(length % 0x100);
                b100 = (byte)((length / 0x100) % 0x100);
                b10000 = (byte)((length / 0x10000) % 0x100);
                b1000000 = (byte)((length / 0x1000000) % 0x100);

                List<byte> r = new List<byte>(new byte[] { 0, 0, 0, 0, b1, b100, b10000, b1000000, (byte)m.MessageType });
                r.AddRange(ms.ToArray());
                ret = r.ToArray();
            }

            return ret;
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
            this.OnReceivedLogEntry(new StringEventArgs(new[] { "Started connection search..." }));
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

            while (true)
            {
                try
                {
                    this.ConnectionClient.Connect(pt);
                    this.ConnectedEndPoint = pt;
                    this.OnConnected(EventArgs.Empty);
                    this.OnReceivedLogEntry(new StringEventArgs(new[]
                    {
                        string.Format(
                        "Connected to {0}:{1}",
                        this.ConnectedEndPoint.Address,
                        this.ConnectedEndPoint.Port)
                    }));
                    break;
                }
                catch (SocketException)
                {
                    Thread.Sleep(5);
                }
            }
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            this.SendMessage(new KeepAlive() { Terminate = true });
            this.ConnectionClient.Close();
            this.OnDisconnected(EventArgs.Empty);
            this.OnReceivedLogEntry(new StringEventArgs(new[]
            {
                string.Format(
                "Disconnected from {0}:{1}",
                this.ConnectedEndPoint.Address,
                this.ConnectedEndPoint.Port)
            }));
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
        public void SendMessage(Message msg)
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
            mes.AddRange(new byte[] { 0, 0, 0, 0, b1, b100, b10000, b1000000, (byte)msg.MessageType });
            mes.AddRange(ms.ToArray());

            if (this.ConnectionClient.Connected && this.ConnectionClient.GetStream().CanWrite)
            {
                try
                {
                    this.ConnectionClient.GetStream().Write(mes.ToArray(), 0, mes.Count);
                }
                catch (IOException exc)
                {
                    this.OnReceivedLogEntry(new StringEventArgs(new[] { exc.Message }));
                }
            }
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
                if (DateTime.Now.Subtract(lastSend).TotalSeconds > 20)
                {
                    try
                    {
                        args.Client.SendDiscover();
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
                        args.Client.OnReceivedLogEntry(new StringEventArgs(new[] { "Trying to connect..." }));
                        args.Client.ConnectTo(a.ServerIP);
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

            while (!args.Stopped)
            {
                // send keep alive
                if (DateTime.Now.Subtract(lastKeepAlive).TotalSeconds >= 10)
                {
                    args.Client.OnReceivedLogEntry(new StringEventArgs(new[] { "Keep Alive" }));
                    lastKeepAlive = DateTime.Now;
                    args.Client.SendMessage(new KeepAlive() { CPUWorkload = (int)cpu.NextValue() });
                }

                if (str.DataAvailable)
                {
                    byte[] hdr = new byte[9];

                    int hlen = str.Read(hdr, 0, 9);
                    uint bodylen;
                    StatusCode messagType;

                    // go to next iteration if header lengh != 9
                    if (!ParseHeader(hdr, out bodylen, out messagType))
                    {
                        continue;
                    }

                    byte[] body = new byte[bodylen];

                    do
                    {
                        Thread.Sleep(50);
                    }
                    while (args.Client.ConnectionClient.Available < bodylen);

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
        /// Sends a UDP broadcast message indicating that the client searches for a server.
        /// </summary>
        private void SendDiscover()
        {
            List<byte> mes = new List<byte>();
            MemoryStream ms = new MemoryStream();

            AgentDiscover d = new AgentDiscover();

            this.formatter.Serialize(ms, d);

            uint length = (uint)ms.Length;
            byte b1, b100, b10000, b1000000;

            b1 = (byte)(length % 0x100);
            b100 = (byte)((length / 0x100) % 0x100);
            b10000 = (byte)((length / 0x10000) % 0x100);
            b1000000 = (byte)((length / 0x1000000) % 0x100);

            mes.AddRange(new byte[] { 0, 0, 0, 0, b1, b100, b10000, b1000000, (byte)StatusCode.AgentConnection });
            mes.AddRange(ms.ToArray());

            UdpBroadcast.SendBoadcast(1233, mes.ToArray());
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