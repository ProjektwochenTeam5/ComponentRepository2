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

        public double AllClientLoad { get; set; }

        public TCPServerManager()
        {
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
            this.MyTCPServer.SendMessage(this.BuildInfos(), clientID);
        }

        private byte[] BuildInfos()
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

            ///////////////// Componentlist fertig

            SendComponentInfos sendcompinfos = new SendComponentInfos();
            sendcompinfos.MetadataComponents = componenlist;

            byte[] senddata = DataConverter.ConvertMessageToByteArray(6, DataConverter.ConvertObjectToByteArray(sendcompinfos));

            return senddata;
        }

        public void RunMyServer()
        {
            this.Dlls = new Dictionary<Guid, string>();
            this.MyTCPServer.Run();
            this.MyTCPServer.Wrapper.GetAssemblies();

            foreach (var item in this.MyTCPServer.Wrapper.Data)
            {
                this.Dlls.Add(new Guid(), item.Location);
            }
        }

        private void AddDllToDictionary(string filename)
        {
            string path = this.MyTCPServer.Wrapper.StorePath + "\\" + filename + ".dll";
            this.Dlls.Add(new Guid(), path);
        }

        private void MyTCPServer_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch (e.MessageType)
            {
                case ClientServerCommunication.StatusCode.KeepAlive:
                    {
                        KeepAlive keepAlive = DataConverter.ConvertByteArrrayToKeepAlive(e.MessageBody);
                        this.CalculateClientLoads(keepAlive);
                        this.CheckIfDeleteClientAndDelete(keepAlive, e.Info);
                        break;
                    }

                case ClientServerCommunication.StatusCode.TransferComponent:
                    break;
                case ClientServerCommunication.StatusCode.TransferJob:
                    break;
                case ClientServerCommunication.StatusCode.DoJobRequest:
                    break;

                case ClientServerCommunication.StatusCode.StorComponent:
                    {
                        StoreComponent storecomponent = DataConverter.ConvertByteArrayToStoreComponent(e.MessageBody);
                        DataBaseWrapper db = new DataBaseWrapper();
                        bool store = db.StoreComponent(storecomponent.Component, e.Info.FriendlyName);

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

        private void SendInfosToAllClients()
        {
            this.MyTCPServer.SendMessageToAll(this.BuildInfos());
        }

        private void CalculateClientLoads(KeepAlive keepAlive)
        {
            Console.WriteLine("Keep alive recieved || Terminate = {0}", keepAlive.Terminate.ToString());
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
