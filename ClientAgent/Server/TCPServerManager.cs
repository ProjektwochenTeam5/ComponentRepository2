namespace Server
{
    using ClientServerCommunication;
    using Core.Component;
    using Core.Network;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class TCPServerManager
    {
        public TCPServer MyTCPServer { get; set; }

        public Dictionary<Guid, double> CPULoads { get; set; }

        public Dictionary<Guid, string> Dlls { get; set; }

        public Dictionary<Guid, Component> Components { get; set; }

        public double AllClientLoad { get; set; }

        public TCPServerManager()
        {
            this.Components = new Dictionary<Guid, Component>();
            this.CPULoads = new Dictionary<Guid, double>();
            this.MyTCPServer = new TCPServer();
            this.MyTCPServer.OnMessageRecieved += this.MyTCPServer_OnMessageRecieved;
            this.MyTCPServer.OnClientFetched += this.MyTCPServer_OnClientFetched;
        }

        private void MyTCPServer_OnClientFetched(object sender, ClientFetchedEventArgs e)
        {
            this.SendComponentInfosToClient(e.ClientId);
        }

        private void SendComponentInfosToClient(Guid clientID)
        {
            this.MyTCPServer.SendMessage(this.BuildComponentInfos(), clientID);
        }

        private byte[] BuildComponentInfos()
        {
            this.MyTCPServer.Wrapper.GetAssemblies();
            Dictionary<IComponent, Guid> componentdic = new Dictionary<IComponent, Guid>();

            foreach (var item in this.MyTCPServer.Wrapper.Data)
            {
                Guid g = default(Guid);

                foreach (var keyvaluepair in this.Dlls)
                {
                    if (keyvaluepair.Value == item.Location)
                    {
                        g = keyvaluepair.Key;
                        break;
                    }
                }

                componentdic.Add(this.MyTCPServer.Wrapper.ReadComponentInfoFormDll(item), g);
            }

            /////////////////// IComponentdictionary fertig

            List<Component> componentlist = new List<Component>();

            foreach (var item in componentdic)
            {
                componentlist.Add(DataConverter.MapIComponentToNetworkComponent(item.Key, item.Value));
            }

            foreach (var item in componentlist)
            {
                if (!this.Components.Keys.Contains(item.ComponentGuid))
                {
                    this.Components.Add(item.ComponentGuid, item);
                }
            }

            ///////////////// Componentlist fertig

            SendComponentInfos sendcompinfos = new SendComponentInfos();
            sendcompinfos.MetadataComponents = componentlist;

            byte[] senddata = DataConverter.ConvertMessageToByteArray(6, DataConverter.ConvertObjectToByteArray(sendcompinfos));

            return senddata;
        }

        public void RunMyServer()
        {
            this.MyTCPServer.Wrapper.GetAssemblies();
            this.Dlls = new Dictionary<Guid, string>();

            foreach (var item in this.MyTCPServer.Wrapper.Data)
            {
                Guid g;
                this.Dlls.Add(g = Guid.NewGuid(), item.Location);
            }

            this.MyTCPServer.Run();
        }

        private void AddDllToDictionary(string filename)
        {
            string path = this.MyTCPServer.Wrapper.StorePath + "\\" + filename + ".dll";
            Guid g;
            this.Dlls.Add(g = Guid.NewGuid(), path);
        }

        private void MyTCPServer_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch (e.MessageType)
            {
                case ClientServerCommunication.StatusCode.KeepAlive:
                    {
                        KeepAlive keepAlive = (KeepAlive)DataConverter.ConvertByteArrayToMessage(e.MessageBody);
                        Console.WriteLine("Keep alive recieved || Terminate = {0} || Workload = {1}", keepAlive.Terminate.ToString(), keepAlive.CPUWorkload.ToString());
                        this.CalculateClientLoads(keepAlive, e.Info.ClientGuid);
                        this.CheckIfDeleteClientAndDelete(keepAlive, e.Info);
                        break;
                    }

                case ClientServerCommunication.StatusCode.TransferComponent:
                    {
                        TransferComponentRequest request = (TransferComponentRequest)DataConverter.ConvertByteArrayToMessage(e.MessageBody);
                        bool contains = this.Components.Keys.Contains(request.ComponentID);

                        if (contains)
                        {
                            this.GiveTranserComponentResponse(e.Info, request.ComponentID, request.MessageID);
                        }
                        else
                        {
                            Console.WriteLine("We don't have this component in store!");
                        }

                        break;                        
                    }

                case ClientServerCommunication.StatusCode.TransferJob:
                    break;
                case ClientServerCommunication.StatusCode.DoJobRequest:
                    {
                        DoJobRequest request = (DoJobRequest)DataConverter.ConvertByteArrayToMessage(e.MessageBody);
                        SplitJob.Split(request);

                        break;
                    }

                case ClientServerCommunication.StatusCode.StorComponent:
                    {
                        StoreComponent storecomponent = (StoreComponent)DataConverter.ConvertByteArrayToMessage(e.MessageBody);
                        DataBaseWrapper db = new DataBaseWrapper();
                        bool store = db.StoreComponent(storecomponent.Component, storecomponent.FriendlyName);

                        if (store)
                        {
                            this.MyTCPServer.SendAck(e.Info, storecomponent.MessageID);
                            this.AddDllToDictionary(e.Info.FriendlyName);
                            this.SendInfosToAllClients();
                        }
                        else
                        {
                            this.MyTCPServer.SendError(e.Info, storecomponent.MessageID);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        private void GiveTranserComponentResponse(ClientInfo clientInfo, Guid guid, int requestid)
        {
            string path = string.Empty;
            try
            {
                 path = this.Dlls[guid];

            }
            catch (Exception)
            {
                Console.WriteLine("We don't have this dll");
                return;
            }

            byte[] data = DataConverter.ConvertDllToByteArray(path);
            TransferComponentResponse response = new TransferComponentResponse();
            response.Component = data;
            response.BelongsTo = requestid;

            byte[] body = DataConverter.ConvertObjectToByteArray(response);

            byte[] send = DataConverter.ConvertMessageToByteArray(4, body);

            this.MyTCPServer.SendMessage(send, clientInfo.ClientGuid);
        }

        private void SendInfosToAllClients()
        {
            this.MyTCPServer.SendMessageToAll(this.BuildComponentInfos());
        }

        private void CalculateClientLoads(KeepAlive keepAlive, Guid clientId)
        {
            this.CPULoads[clientId] = keepAlive.CPUWorkload;
        }

        private void CheckIfDeleteClientAndDelete(KeepAlive ka, ClientInfo info)
        {
            if (ka.Terminate)
            {
                this.MyTCPServer.Clients.Remove(info);
                Console.WriteLine("Client deleted!");
            }
            else
            {
            }
        }

        public ICollection<Component> componenlist { get; set; }
    }
}
