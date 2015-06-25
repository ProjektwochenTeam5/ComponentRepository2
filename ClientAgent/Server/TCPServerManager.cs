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
            this.CPULoads = new Dictionary<Guid, int>();
            this.MyTCPServer = new TCPServer();
            this.MyTCPServer.OnMessageRecieved += this.MyTCPServer_OnMessageRecieved;
            this.MyTCPServer.OnClientFetched += this.MyTCPServer_OnClientFetched;
            this.ServerGuid = Guid.NewGuid();
            this.AllServerComponents = new Dictionary<Guid, List<Component>>();
            this.ClientPing = new Dictionary<Guid, uint>();
            this.ComplexComponent = new Dictionary<Guid, string>();

        }

        public TCPServerManager(RecBroadcast broadcast)
        {
            this.IpAdressFriendlyName = new Dictionary<string, string>();
            this.JobsQueued = new List<Guid>();
            this.Components = new Dictionary<Guid, Component>();
            this.CPULoads = new Dictionary<Guid, int>();
            this.MyTCPServer = new TCPServer();
            this.MyTCPServer.OnMessageRecieved += this.MyTCPServer_OnMessageRecieved;
            this.MyTCPServer.OnClientFetched += this.MyTCPServer_OnClientFetched;
            this.ServerGuid = Guid.NewGuid();
            broadcast.OnUdpClientDiscovered += broadcast_OnUdpClientDiscovered;
            this.AllServerComponents = new Dictionary<Guid, List<Component>>();
            this.ClientPing = new Dictionary<Guid, uint>();
            this.ComplexComponent = new Dictionary<Guid, string>();
        }

        public object locker = new object();

        public TCPServer MyTCPServer { get; set; }

        public event EventHandler<JobResponseRecievedEventArgs> OnJobResponseRecieved;

        public event EventHandler<ClientFetchedEventArgs> OnClientDisconnected;

        public event EventHandler<GetAssemblyEventArgs> OnAssemblyNotInStore;

        public event EventHandler<ComponentSubmitEventArgs> OnComponentSubmitted;

        public event EventHandler<JobEventArgs> OnJobReadyToExecute;

        public Dictionary<Guid, int> CPULoads { get; set; }

        public Dictionary<Guid,List<Component>> AllServerComponents { get; set; }

        public Dictionary<Guid, List<ClientInfo>> AllServerClients { get; set; }

        public Dictionary<Guid, int> AllServerCpuLoads { get; set; }

        public Dictionary<Guid, uint> ClientPing { get; set; }

        public Dictionary<Guid, string> Dlls { get; set; }

        public Dictionary<Guid, Component> Components { get; set; }

        public Dictionary<Guid, string> ComplexComponent { get; set; }

        public Dictionary<string, string> IpAdressFriendlyName { get; set; }

        public List<Guid> JobsQueued { get; set; }

        public Guid ServerGuid { get; set; }

        public int ServerCpuLoad 
        { 
            get
            {
                var load = 0;
                foreach (var item in this.CPULoads)
                {
                    load += item.Value;
                }

                return load / this.CPULoads.Count;
            }
            set; 
        }

        public double AllClientLoad { get; set; }

        void broadcast_OnUdpClientDiscovered(object sender, UdpClientDiscoverRecievedEventArgs e)
        {
            if (this.IpAdressFriendlyName.Keys.Contains(e.IPAdress))
            {
                return;
            }
            this.IpAdressFriendlyName.Add(e.IPAdress, e.FriendlyName);
        }

        private void MyTCPServer_OnClientFetched(object sender, ClientFetchedEventArgs e)
        {
            this.SendComponentInfosToClient(e.ClientInfo);
            e.ClientInfo.FriendlyName = this.IpAdressFriendlyName[e.ClientInfo.IpAddress.ToString()];

            this.ClientPing.Add(e.ClientInfo.ClientGuid, 0);

            Task t = new Task(new Action(() =>
            {
                while (true)
                {
                    uint old = this.ClientPing[e.ClientInfo.ClientGuid];
                    Thread.Sleep(60000);
                    if (old == this.ClientPing[e.ClientInfo.ClientGuid])
                    {
                        try
                        {
                            this.MyTCPServer.Clients[e.ClientInfo].Close();
                            this.IpAdressFriendlyName.Remove(e.ClientInfo.IpAddress.ToString());
                            this.OnClientDisconnected(this, new ClientFetchedEventArgs(e.ClientInfo));
                            this.MyTCPServer.Clients.Remove(e.ClientInfo);
                            Console.WriteLine("60 seconds over - Client deleted!");
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                    }
                }
            }));

            t.Start();
        }

        private void IncrementClientKeepAlive(Guid clientguid)
        {
            this.ClientPing[clientguid] = this.ClientPing[clientguid] + 1;
        }

        private void SendComponentInfosToClient(ClientInfo info)
        {
            this.MyTCPServer.SendMessage(this.BuildComponentInfos(), info.ClientGuid);
        }

        private byte[] BuildComponentInfos()
        {
            this.MyTCPServer.Wrapper.ReadData();
            Dictionary<IComponent, Guid> componentdic = new Dictionary<IComponent, Guid>();

            foreach (var item in this.MyTCPServer.Wrapper.AtomicData)
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

            foreach (var item in this.MyTCPServer.Wrapper.ComplexData)
            {
                componentlist.Add(this.MyTCPServer.Wrapper.ReadInfosFromDatFile(item.Key));
            }


            ///////////////// Componentlist fertig

            SendComponentInfos sendcompinfos = new SendComponentInfos();
            sendcompinfos.MetadataComponents = componentlist;

            byte[] senddata = DataConverter.ConvertMessageToByteArray(6, DataConverter.ConvertObjectToByteArray(sendcompinfos));


            foreach (var item in componentlist)
            {
                if (!this.Components.Keys.Contains(item.ComponentGuid))
                {
                    this.Components.Add(item.ComponentGuid, item);
                }
            }

            return senddata;
        }

        public void RunMyServer()
        {
            this.MyTCPServer.Wrapper.ReadData();
            this.Dlls = new Dictionary<Guid, string>();

            foreach (var item in this.MyTCPServer.Wrapper.AtomicData)
            {
                Guid g;
                this.Dlls.Add(g = Guid.NewGuid(), item.Value);
            }

            foreach (var item in this.MyTCPServer.Wrapper.ComplexData)
            {
                this.ComplexComponent.Add(Guid.NewGuid(), item.Key);
            }

            this.MyTCPServer.Wrapper.ReadData();
            Dictionary<IComponent, Guid> componentdic = new Dictionary<IComponent, Guid>();

            foreach (var item in this.MyTCPServer.Wrapper.AtomicData)
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

            foreach (var item in this.MyTCPServer.Wrapper.ComplexData)
            {
                componentlist.Add(this.MyTCPServer.Wrapper.ReadInfosFromDatFile(item.Key));
            }

            foreach (var item in componentlist)
            {
                if (!this.Components.Keys.Contains(item.ComponentGuid))
                {
                    this.Components.Add(item.ComponentGuid, item);
                }
            }

            this.MyTCPServer.Run();
        }

        public List<object> SendJobToClient(Guid componentGuid, List<object> inputData, Guid jobGuid)
        {
            TransferJobRequest req = new TransferJobRequest();
            req.ServerID = this.ServerGuid;
            req.InputData = inputData;
            req.ComponentGuid = componentGuid;
            req.JobID = jobGuid;
            

            if (req.InputData == null)
            {
                req.InputData = new List<object>();
            }

            this.JobsQueued.Add(jobGuid);

            bool waiting = true;
            TransferJobResponse resp = null;

            EventHandler<JobResponseRecievedEventArgs> d = delegate(object sender, JobResponseRecievedEventArgs e)
            {
                if (e.BelongsToJob == jobGuid)
                {
                    try
                    {
                        Console.WriteLine("Answer from Client matches Request! Result is: " + e.Data.Result.ElementAt(0));
                        resp = e.Data;
                        waiting = false;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            };

            this.OnJobResponseRecieved += d;

            var sendData = DataConverter.ConvertMessageToByteArray(5, DataConverter.ConvertObjectToByteArray(req));

            Guid client = new Guid();
            lock (this.locker)
            {
                int smallest = 100;
                foreach (var item in this.CPULoads)
                {
                    if (item.Value <= smallest)
                    {
                        smallest = item.Value;
                        client = item.Key;
                    }
                }
            }

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

        ///////////////////////////////////////////////// ON MESSAGE RECIEVED /////////////////////////////////////////////////
        
        /// <summary>
        /// On Message recieved callback function.
        /// </summary>
        /// <param name="sender">The owner of the event.</param>
        /// <param name="e">Some arguments.</param>
        private void MyTCPServer_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch (e.MessageType)
            {
                case ClientServerCommunication.StatusCode.KeepAlive:
                    {
                        KeepAlive keepAlive = (KeepAlive)DataConverter.ConvertByteArrayToMessage(e.MessageBody);
                        Console.WriteLine("Keep alive recieved from {0} || Terminate = {1} || Workload = {2}",e.Info.FriendlyName, keepAlive.Terminate.ToString(), keepAlive.CPUWorkload.ToString());
                        this.CalculateClientLoads(keepAlive, e.Info.ClientGuid);
                        this.IncrementClientKeepAlive(e.Info.ClientGuid);
                        this.CheckIfDeleteClientAndDelete(keepAlive, e.Info);
                        break;
                    }

                case ClientServerCommunication.StatusCode.TransferComponent:
                    {
                        TransferComponentRequest request = (TransferComponentRequest)DataConverter.ConvertByteArrayToMessage(e.MessageBody);
                        
                        bool contains = this.Components.Keys.Contains(request.ComponentID);
                        bool containscomplex = this.ComplexComponent.Keys.Contains(request.ComponentID);

                        Console.WriteLine("--------Transfer component request Recieved");
                        if (contains)
                        {
                            this.GiveTransferComponentResponse(e.Info, request.ComponentID, request.MessageID);
                        }
                        else if (containscomplex)
                        {
                            this.GiveTransferComponentResponseIfComplex(e.Info, request.ComponentID, request.MessageID);
                        }
                        else
                        {
                            this.FireOnAssemblyNotInStore(new GetAssemblyEventArgs(e.Info, request.MessageID, request.ComponentID));
                            //Console.WriteLine("We don't have this component in store!");
                            //this.MyTCPServer.SendError(e.Info, request.MessageID);
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
                        var targetServer = this.AllServerCpuLoads.OrderBy(x => x.Value).First();
                        if (this.ServerCpuLoad < targetServer.Value)
                        {
                            Task t = new Task(new Action(() =>
                            SplitJob.Split(request, this)));

                            t.Start();
                        }
                        else
                        {
                            this.OnJobReadyToExecute(this, new JobEventArgs(request.Job, request.TargetDisplay, request.TargetDisplay, new List<object>(), request.Job.FriendlyName, targetServer.Key));
                        }

                        break;
                    }

                case ClientServerCommunication.StatusCode.StorComponent:
                    {
                        Console.WriteLine("StoreComponent recieved!");
                        StoreComponent storecomponent = (StoreComponent)DataConverter.ConvertByteArrayToMessage(e.MessageBody);

                        if (!storecomponent.IsComplex)
                        {
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
                        }
                        else
                        {
                            this.ComplexComponent.Add(Guid.NewGuid(), storecomponent.FriendlyName);
                            this.MyTCPServer.Wrapper.StoreComplexComponent(storecomponent.Component, storecomponent.FriendlyName);
                        }

                        Component c = null;
                        foreach (var item in this.Components)
                        {
                            if (item.Value.FriendlyName == e.Info.FriendlyName)
                            {
                                c = item.Value;
                            }
                        }

                        if (c != null)
                        {
                            this.FireOnComponentSubmitted(new ComponentSubmitEventArgs(c));
                        }

                        break;
                    }
                default:
                    break;
            }
        }

        ///////////////////////////////////////////////// ON MESSAGE RECIEVED /////////////////////////////////////////////////

        public void GiveTransferComponentResponse(ClientInfo clientInfo, Guid guid, Guid requestid)
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
            Console.WriteLine("sending: " + path);
            this.MyTCPServer.SendMessage(send, clientInfo.ClientGuid);
        }

        public void GiveTransferComponentResponseIfComplex(ClientInfo clientInfo, Guid guid, Guid requestid)
        {
            string name = string.Empty;
            try
            {
                name = this.ComplexComponent[guid];

            }
            catch (Exception)
            {
                Console.WriteLine("We don't have this dll");
                return;
            }

            byte[] data = this.MyTCPServer.Wrapper.GetComplexComponent(name);
            TransferComponentResponse response = new TransferComponentResponse();
            response.Component = data;
            response.BelongsTo = requestid;

            byte[] body = DataConverter.ConvertObjectToByteArray(response);

            byte[] send = DataConverter.ConvertMessageToByteArray(4, body);
            Console.WriteLine("sending: " + name);
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
                this.MyTCPServer.Clients[info].Close();
                this.MyTCPServer.Clients.Remove(info);
                this.OnClientDisconnected(this, new ClientFetchedEventArgs(info));
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

        public void FireOnClientDisconnected(ClientFetchedEventArgs e)
        {
            if (this.OnClientDisconnected != null)
            {
                this.OnClientDisconnected(this, e);
            }
        }

        public void FireOnAssemblyNotInStore(GetAssemblyEventArgs e)
        {
            if (this.OnAssemblyNotInStore != null)
            {
                this.OnAssemblyNotInStore(this, e);
            }
        }

        public void FireOnComponentSubmitted(ComponentSubmitEventArgs e)
        {
            if (this.OnComponentSubmitted != null)
            {
                this.OnComponentSubmitted(this, e);
            }
        }

        public void FireOnJobReadyToExecute(JobEventArgs e)
        {
            if (this.OnJobReadyToExecute != null)
            {
                this.OnJobReadyToExecute(this, e);
            }
        }
    }
}
