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
    using System.Threading;
    using System.Threading.Tasks;

    public class TCPServerManager
    {
        public TCPServerManager()
        {
            this.IpAdressFriendlyName = new Dictionary<string, string>();
            this.JobsQueued = new List<Guid>();
            this.Components = new Dictionary<Guid, Component>();
            this.CPULoads = new Dictionary<Guid, double>();
            this.MyTCPServer = new TCPServer();
            this.MyTCPServer.OnMessageRecieved += this.MyTCPServer_OnMessageRecieved;
            this.MyTCPServer.OnClientFetched += this.MyTCPServer_OnClientFetched;
            this.ServerGuid = Guid.NewGuid();
            this.AllServerComponents = new Dictionary<Guid, List<Component>>();
        }

        public TCPServerManager(RecBroadcast broadcast)
        {
            this.IpAdressFriendlyName = new Dictionary<string, string>();
            this.JobsQueued = new List<Guid>();
            this.Components = new Dictionary<Guid, Component>();
            this.CPULoads = new Dictionary<Guid, double>();
            this.MyTCPServer = new TCPServer();
            this.MyTCPServer.OnMessageRecieved += this.MyTCPServer_OnMessageRecieved;
            this.MyTCPServer.OnClientFetched += this.MyTCPServer_OnClientFetched;
            this.ServerGuid = Guid.NewGuid();
            broadcast.OnUdpClientDiscovered += broadcast_OnUdpClientDiscovered;
            this.AllServerComponents = new Dictionary<Guid, List<Component>>();
        }

        public TCPServer MyTCPServer { get; set; }

        public event EventHandler<JobResponseRecievedEventArgs> OnJobResponseRecieved;

        public Dictionary<Guid, double> CPULoads { get; set; }

        public Dictionary<Guid,List<Component>> AllServerComponents { get; set; }

        public Dictionary<Guid, List<ClientInfo>> AllServerClients { get; set; }

        public Dictionary<Guid, string> Dlls { get; set; }

        public Dictionary<Guid, Component> Components { get; set; }

        public Dictionary<string, string> IpAdressFriendlyName { get; set; }

        public List<Guid> JobsQueued { get; set; }

        public Guid ServerGuid { get; set; }

        public double AllClientLoad { get; set; }

        void broadcast_OnUdpClientDiscovered(object sender, UdpClientDiscoverRecievedEventArgs e)
        {
            this.IpAdressFriendlyName.Add(e.IPAdress, e.FriendlyName);
        }

        private void MyTCPServer_OnClientFetched(object sender, ClientFetchedEventArgs e)
        {
            this.SendComponentInfosToClient(e.ClientInfo);
            e.ClientInfo.FriendlyName = this.IpAdressFriendlyName[e.ClientInfo.IpAddress.ToString()];
        }

        private void SendComponentInfosToClient(ClientInfo info)
        {
            this.MyTCPServer.SendMessage(this.BuildComponentInfos(), info.ClientGuid);
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
                    if (keyvaluepair.Value == item.Value)
                    {
                        g = keyvaluepair.Key;
                        break;
                    }
                }

                componentdic.Add(this.MyTCPServer.Wrapper.ReadComponentInfoFormDll(item.Key), g);
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
                this.Dlls.Add(g = Guid.NewGuid(), item.Value);
            }

            this.MyTCPServer.Run();
        }

        public List<object> SendJobToClient(Guid componentGuid, List<object> inputData, Guid jobGuid)
        {
            TransferJobRequest req = new TransferJobRequest();
            req.ServerID = this.ServerGuid;
            req.InputData = inputData;
            req.InputData = new List<object>() { 4, 3 };
            req.ComponentGuid = componentGuid;

            this.JobsQueued.Add(jobGuid);

            bool waiting = true;
            TransferJobResponse resp = null;

            EventHandler<JobResponseRecievedEventArgs> d = delegate(object sender, JobResponseRecievedEventArgs e)
            {
                if (e.BelongsToJob == jobGuid)
                {
                    Console.WriteLine("Answer from Client matches Request! Result is: " + e.Data.Result.ElementAt(0));
                    resp = e.Data;
                    waiting = false;
                }
            };

            this.OnJobResponseRecieved += d;

            var sendData = DataConverter.ConvertMessageToByteArray(5, DataConverter.ConvertObjectToByteArray(req));
            var client = this.CPULoads.Where(y => y.Value == CPULoads.Min(x => x.Value)).Single().Key;
            this.MyTCPServer.SendMessage(sendData, client);

            Console.WriteLine("Sending TransferJobReqest!");

            try
            {
                while (waiting)
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
                this.OnJobResponseRecieved -= d;
            }

            return resp.Result.ToList();
        }

        private void AddDllToDictionary(string filename)
        {
            this.Dlls.Add(Guid.NewGuid(), this.MyTCPServer.Wrapper.StorePath + "\\" + filename + ".dll");
        }

        private void MyTCPServer_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch (e.MessageType)
            {
                case ClientServerCommunication.StatusCode.KeepAlive:
                    {
                        KeepAlive keepAlive = (KeepAlive)DataConverter.ConvertByteArrayToMessage(e.MessageBody);
                        Console.WriteLine("Keep alive recieved from {0} || Terminate = {1} || Workload = {2}",e.Info.FriendlyName, keepAlive.Terminate.ToString(), keepAlive.CPUWorkload.ToString());
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
                            this.MyTCPServer.SendError(e.Info, request.MessageID);
                        }

                        break;                        
                    }

                case ClientServerCommunication.StatusCode.TransferJob:
                    {
                        TransferJobResponse response = (TransferJobResponse)DataConverter.ConvertByteArrayToMessage(e.MessageBody);
                        this.FireOnJobResponseRecieved(new JobResponseRecievedEventArgs(response.BelongsToRequest, response));

                        break;
                    }

                case ClientServerCommunication.StatusCode.DoJobRequest:
                    {
                        DoJobRequest request = (DoJobRequest)DataConverter.ConvertByteArrayToMessage(e.MessageBody);
                        this.MyTCPServer.SendAck(e.Info, request.MessageID);

                        Console.WriteLine("DoJobRequest recieved!");

                        Task t = new Task(new Action(() =>
                        SplitJob.Split(request, this)));

                        t.Start();

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
                this.IpAdressFriendlyName.Remove(info.IpAddress.ToString());
                Console.WriteLine("Client deleted!");
            }
            else
            {
            }
        }

        protected void FireOnJobResponseRecieved(JobResponseRecievedEventArgs e)
        {
            if (this.OnJobResponseRecieved != null)
            {
                this.OnJobResponseRecieved(this, e);
            }
        }

    }
}
