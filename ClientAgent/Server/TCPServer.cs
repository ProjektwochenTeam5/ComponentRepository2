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

    /// <summary>
    /// Represnets the TCPServer class.
    /// </summary>
    public class TCPServer
    {
        public TcpListener MyListener { get; set; }

        public List<TcpClient> Clients { get; set; }

        public event EventHandler<MessageRecievedEventArgs> OnMessageRecieved;

        public TCPServer()
        {
            this.MyListener = new TcpListener(IPAddress.Any, 12345);
            this.Clients = new List<TcpClient>();
        }

        public void Run()
        {
            try
            {
                this.MyListener.Start();

                while (true)
                {
                    TcpClient client = this.MyListener.AcceptTcpClient();

                    Thread clientThread = new Thread(new ParameterizedThreadStart(ClientWorker));
                    clientThread.Start(client);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ClientWorker(object obj)
        {
            TcpClient client = (TcpClient)obj;
            this.Clients.Add(client);
            NetworkStream ns = client.GetStream();

            this.SendAckToClient(ns, 3);

            while (true)
            {
                bool cont = true;
                byte[] body = null;
                byte messagetype = 0;

                if (ns.DataAvailable)
                {
                    int index = 0;
                    while (cont)
                    {
                        byte[] buffer = new byte[1024];

                        int recievedbytes = ns.Read(buffer, 0, buffer.Length);

                        if (body == null)
                        {
                            byte[] messageLength = new byte[4];

                            for (int i = 0; i < 4; i++)
                            {
                                messageLength[i] = buffer[4 + i];
                            }

                            var length = BitConverter.ToInt32(messageLength, 0) + 9;
                            messagetype = buffer[8];
                            body = new byte[length - 9];

                            for (int i = 9; i < recievedbytes; i++)
                            {
                                body[index] = buffer[i];
                                index++;
                            }

                            continue;
                        }

                        for (int i = 0; i < recievedbytes; i++)
                        {
                            body[index] = buffer[i];
                            index++;
                        }

                        if (index >= body.Length)
                        {
                            this.FireOnMessageRecieved(new MessageRecievedEventArgs(body, (int)messagetype));
                            break;
                        }
                    }
                }
            }
        }

        private void SendAckToClient(NetworkStream ns, int id)
        {
            Acknowledge ack = new Acknowledge();
            ack.MessageID = id;

            var send = DataConverter.ConvertMessageToByteArray(3, DataConverter.ConvertObjectToByteArray(ack));

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

    }
}
