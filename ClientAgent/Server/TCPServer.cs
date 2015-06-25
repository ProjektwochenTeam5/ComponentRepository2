// ----------------------------------------------------------------------- 
// <copyright file="TCPServer.cs" company="Gunter Wiesinger"> 
// Copyright (c) Gunter Wiesinger. All rights reserved. 
// </copyright> 
// <summary>This application is just for testing purposes.</summary> 
// <author>Gunter Wiesinger/Auto generated</author> 
// -----------------------------------------------------------------------
namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ClientServerCommunication;
    using Core.Network;
    using Core.Component;
    using System.IO;

    /// <summary>
    /// Represnets the TCPServer class.
    /// </summary>
    public class TCPServer
    {
        public TcpListener MyListener { get; set; }

        public DataBaseWrapper Wrapper { get; set; }

        public Dictionary<ClientInfo, TcpClient> Clients { get; set; }

        public event EventHandler<MessageRecievedEventArgs> OnMessageRecieved;

        public event EventHandler<ClientFetchedEventArgs> OnClientFetched;
        
        public TCPServer()
        {
            this.MyListener = new TcpListener(IPAddress.Any, 12345);
            this.Clients = new Dictionary<ClientInfo, TcpClient>();
            this.Wrapper = new DataBaseWrapper();
        }

        void MessageManager_OnClientWantsToQuit(object sender, ClientTerminatedEventArgs e)
        {
            this.Clients.Remove(e.ClientWhoWantsToQuit);
        }

        public void Run()
        {
            try
            {
                this.MyListener.Start();
                Console.WriteLine("--> TCP Listener started");

                while (true)
                {
                    TcpClient client = this.MyListener.AcceptTcpClient();
                    Thread clientThread = new Thread(new ParameterizedThreadStart(ClientWorker));
                    clientThread.IsBackground = true;
                    clientThread.Start(client);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine("exception at starting listener: " + e.Message);
            }
        }

        private void ClientWorker(object obj)
        {
            TcpClient client = (TcpClient)obj;
            ClientInfo clientInfo = new ClientInfo();
            clientInfo.ClientGuid = Guid.NewGuid();
            clientInfo.IpAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
            clientInfo.FriendlyName = "Markus";
            client.SendTimeout = 30;
            this.Clients.Add(clientInfo, client);
            client.SendBufferSize = UInt16.MaxValue * 16;
            client.ReceiveBufferSize = UInt16.MaxValue * 16;
            NetworkStream ns = client.GetStream();

            this.FireOnClientFetched(new ClientFetchedEventArgs(clientInfo));
            Console.WriteLine("----> Client fetched! FriendlyName: {0} - ClientID: {1}", clientInfo.FriendlyName, clientInfo.ClientGuid);

            while (true)
            {
                try
                {
                    if (ns.DataAvailable)
                    {
                        byte[] hdr = new byte[9];

                        int hlen = ns.Read(hdr, 0, 9);
                        uint bodylen;
                        StatusCode messagType;

                        // go to next iteration if header lengh != 9
                        if (!ParseHeader(hdr, out bodylen, out messagType))
                        {
                            continue;
                        }

                        byte[] body = new byte[bodylen];

                        Thread.Sleep(10);
                        int rcvbody = ns.Read(body, 0, (int)bodylen);

                        this.FireOnMessageRecieved(new MessageRecievedEventArgs(body, messagType, clientInfo));
                    }
                }
                catch (ObjectDisposedException e)
                {
                    break;
                }
            }

            try
            {
                client.Close();
                ns.Close();
                ns.Dispose();
            }
            catch (Exception)
            {

            }
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

            if (header[0] != 0 || header[1] != 0 || header[2] != 0 || header[3] != 0)
            {
                return false;
            }

            length = (uint)(header[4] + (header[5] * 0x100) + (header[6] * 0x10000) + (header[7] * 0x1000000));

            status = (StatusCode)header[8];
            return true;
        }

        private void SendAckToClient(NetworkStream ns, Guid belongingmessageid)
        {
            Acknowledge ack = new Acknowledge();
            ack.BelongsTo = belongingmessageid;

            var send = DataConverter.ConvertMessageToByteArray(3, DataConverter.ConvertObjectToByteArray(ack));

            try
            {
                ns.Write(send, 0, send.Length);
                Console.WriteLine("Acknowledge sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ns.Flush();
        }

        private void SendErrorToClient(NetworkStream ns, Guid belongingmessageid)
        {
            Error err = new Error();
            err.BelongsTo = belongingmessageid;
            var send = DataConverter.ConvertMessageToByteArray(9, DataConverter.ConvertObjectToByteArray(err));

            try
            {
                ns.Write(send, 0, send.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void FireOnMessageRecieved(MessageRecievedEventArgs e)
        {
            if (this.OnMessageRecieved != null)
            {
                this.OnMessageRecieved(this, e);
            }
        }

        public void FireOnClientFetched(ClientFetchedEventArgs e)
        {
            if (this.OnClientFetched != null)
            {
                this.OnClientFetched(this, e);
            }
        }

        public void SendAck(ClientInfo clientinfo, Guid belongingmessageid)
        {
            TcpClient client = this.Clients[clientinfo];
            NetworkStream ns = client.GetStream();

            this.SendAckToClient(ns, belongingmessageid);
        }

        public void SendError(ClientInfo clientinfo, Guid belongingmessageid)
        {
            TcpClient client = this.Clients[clientinfo];
            NetworkStream ns = client.GetStream();

            this.SendErrorToClient(ns, belongingmessageid);
        }

        public void SendMessage(byte[] message, Guid clientID)
        {
            TcpClient client = null;
            NetworkStream stream = null;

            try
            {
                 client = this.Clients.Where(x => x.Key.ClientGuid == clientID).SingleOrDefault().Value;
                 stream = client.GetStream();
            }
            catch (Exception)
            {
                if (stream == null || client == null)
                {
                    return;
                }
            }


            try
            {
                stream.Write(message, 0, message.Length);
                stream.Flush();

                Console.WriteLine("Sending Message! + {0} bytes!", message.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendMessageToAll(byte[] message)
        {
            foreach (var item in this.Clients)
            {
                NetworkStream stream = item.Value.GetStream();
                try
                {
                    stream.Write(message, 0, message.Length);
                    stream.Flush();
                    Console.WriteLine("Sending Message!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
