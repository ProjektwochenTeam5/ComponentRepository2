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

    /// <summary>
    /// Represnets the TCPServer class.
    /// </summary>
    public class TCPServer
    {
        public TcpListener MyListener { get; set; }

        public DataBaseWrapper Wrapper { get; set; }

        public Dictionary<ClientInfo, TcpClient> Clients { get; set; }

        public event EventHandler<MessageRecievedEventArgs> OnMessageRecieved;

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
                Console.WriteLine("listener started");

                while (true)
                {
                    TcpClient client = this.MyListener.AcceptTcpClient();
                    Console.WriteLine("Client da :D");
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
            clientInfo.ClientGuid = new Guid();
            clientInfo.IpAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
            clientInfo.FriendlyName = "Markus";
            this.Clients.Add(clientInfo, client);

            NetworkStream ns = client.GetStream();

           // this.SendAckToClient(ns);
            this.SendComponentInfos(ns);

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
                        Console.WriteLine(recievedbytes + " bytes empfangen");

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

                            if (recievedbytes == buffer.Length)
                            {
                                continue;
                            }
                            else
                            {
                                this.FireOnMessageRecieved(new MessageRecievedEventArgs(body, (StatusCode)messagetype, clientInfo));
                                break;
                            }
                        }

                        for (int i = 0; i < recievedbytes; i++)
                        {
                            body[index] = buffer[i];
                            index++;
                        }

                        if (index >= body.Length)
                        {
                            this.FireOnMessageRecieved(new MessageRecievedEventArgs(body, (StatusCode)messagetype, clientInfo));
                            break;
                        }
                    }
                }
            }
        }

        private void SendComponentInfos(NetworkStream ns)
        {
            this.Wrapper.GetAssemblies();
            List<Component> comp = new List<Component>();
            List<IComponent> l = new List<IComponent>();

            foreach (var assembly in this.Wrapper.Data)
            {
                l.Add(this.Wrapper.ReadComponentInfoFormDll(assembly));
            }

            foreach (var icomponent in l)
            {
                comp.Add(DataConverter.MapIComponentToNetworkComponent(icomponent));
            }

            SendComponentInfos sendcompinfos = new SendComponentInfos();
            sendcompinfos.MetadataComponents = comp;

            byte[] senddata = DataConverter.ConvertMessageToByteArray(6, DataConverter.ConvertObjectToByteArray(sendcompinfos));

            try
            {
                ns.Write(senddata, 0, senddata.Length);
                Console.WriteLine("Send Component Infos");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendComponentInfos()
        {
            this.Wrapper.GetAssemblies();
            List<Component> comp = new List<Component>();
            List<IComponent> l = new List<IComponent>();

            foreach (var assembly in this.Wrapper.Data)
            {
                l.Add(this.Wrapper.ReadComponentInfoFormDll(assembly));
            }

            foreach (var icomponent in l)
            {
               comp.Add(DataConverter.MapIComponentToNetworkComponent(icomponent));
            }

            SendComponentInfos sendcompinfos = new SendComponentInfos();
            sendcompinfos.MetadataComponents = comp;

            byte[] senddata = DataConverter.ConvertMessageToByteArray(6, DataConverter.ConvertObjectToByteArray(sendcompinfos));

            foreach (var item in this.Clients.Values)
            {
                NetworkStream ns = item.GetStream();
                try
                {
                    ns.Write(senddata, 0, senddata.Length);
                    Console.WriteLine("Send Component Infos");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void SendAckToClient(NetworkStream ns, int belongingmessageid)
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

        private void SendErrorToClient(NetworkStream ns, int belongingmessageid)
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

        public void SendAck(ClientInfo clientinfo, int belongingmessageid)
        {
            TcpClient client = this.Clients[clientinfo];
            NetworkStream ns = client.GetStream();

            this.SendAckToClient(ns, belongingmessageid);
        }

        public void SendError(ClientInfo clientinfo, int belongingmessageid)
        {
            TcpClient client = this.Clients[clientinfo];
            NetworkStream ns = client.GetStream();

            this.SendErrorToClient(ns, belongingmessageid);
        }

    }
}
