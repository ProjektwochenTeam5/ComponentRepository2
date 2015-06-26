using ClientServerCommunication;
using Core.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class TcpServerToServerManager
    {
        public TcpServerToServerManager(TCPServerManager manager)
        {
            this.Server = new ServerReceiver();

            this.Server.OnMessageRecieved += Server_OnMessageRecieved;
            this.TcpManager = manager;
            this.TcpManager.OnClientDisconnected += TcpManager_OnClientDisconnected;

            this.TcpManager.MyTCPServer.OnClientFetched += MyTCPServer_OnClientFetched;
            this.TcpManager.OnAssemblyNotInStore += TcpManager_OnAssemblyNotInStore;
            this.TcpManager.OnComponentSubmitted += TcpManager_OnComponentSubmitted;
            this.TcpManager.OnJobReadyToExecute += TcpManager_OnJobReadyToExecute;

            Task serverReceiverTask = new Task(() => this.Server.StartReceiving());
            serverReceiverTask.Start();

            KeepAliveServerToServer keepAlive = new KeepAliveServerToServer(this);
            Task keepAliveTask = new Task(() => keepAlive.SendKeepAlives(this.Server.Servers));
            keepAliveTask.Start();
        }

        void TcpManager_OnJobReadyToExecute(object sender, JobEventArgs e)
        {
            JobServerToServer jobExec = new JobServerToServer(this);
            var ipEndPoint = this.Server.Servers.FirstOrDefault(x => x.Key == e.TargetServer).Value;
            Task jobExecTask = new Task(() => jobExec.SendJobRequest(ipEndPoint, e.MyComponent, e.TargetDisplay, e.TargetCalc, e.InputData, e.Friendly));
            jobExecTask.Start();
        }

        void TcpManager_OnComponentSubmitted(object sender, ComponentSubmitEventArgs e)
        {
            ComponentSubmitServerToServer componentSubmit = new ComponentSubmitServerToServer(this);
            Task componentSubmitTask = new Task(() => componentSubmit.SendComponentSubmitRequest(this.Server.Servers, e.MyComponent));
            componentSubmitTask.Start();
        }

        void TcpManager_OnAssemblyNotInStore(object sender, GetAssemblyEventArgs e)
        {
            AssemblyServerToServer assembly = new AssemblyServerToServer(this);
            var ass = assembly.SendAssemblyRequest(this.Server.Servers, e.ComponentID);
            if (ass != null)
            {
                this.TcpManager.GiveTransferComponentResponse(e.Info, e.ComponentID, e.MessageID);
            }
            else
            {
                Console.WriteLine("We don't have this component in store!");
                this.TcpManager.MyTCPServer.SendError(e.Info, e.MessageID);
            }
        }

        void TcpManager_OnClientDisconnected(object sender, ClientFetchedEventArgs e)
        {
            ClientUpdateServerToServer upd = new ClientUpdateServerToServer(this);
            Task clientUpdateTask = new Task(() => upd.SendClientUpdateRequest(this.Server.Servers, e.ClientInfo, ClientState.Disconnected));
            clientUpdateTask.Start();
        }

        void MyTCPServer_OnClientFetched(object sender, ClientFetchedEventArgs e)
        {
            ClientUpdateServerToServer upd = new ClientUpdateServerToServer(this);
            Task clientUpdateTask = new Task(() => upd.SendClientUpdateRequest(this.Server.Servers, e.ClientInfo, ClientState.Connected));
            clientUpdateTask.Start();
        }

        public TCPServerManager TcpManager { get; set; }
        
        public ServerReceiver Server;

        void Server_OnMessageRecieved(object sender, ServerMessageReceivedEventArgs e)
        {
            switch (e.MessageCode)
            {
                case Core.Network.MessageCode.Logon:
                    LogonRequest logonReq = JsonConvert.DeserializeObject<LogonRequest>(Encoding.ASCII.GetString(e.MessageBody));
                    this.Server.Servers.Add(logonReq.ServerGuid, (IPEndPoint)e.Server.Client.RemoteEndPoint);
                    this.SendLogonResponse(e.Server, logonReq.LogonRequestGuid);
                    break;
                case Core.Network.MessageCode.KeepAlive:
                    KeepAliveRequest keepAliveReq = JsonConvert.DeserializeObject<KeepAliveRequest>(Encoding.ASCII.GetString(e.MessageBody));
                    this.SaveKeepAliveData(keepAliveReq);
                    this.SendKeepAliveResponse(e.Server, keepAliveReq.KeepAliveRequestGuid);
                    break;
                case Core.Network.MessageCode.ComponentSubmit:
                    ComponentSubmitRequest submitReq = JsonConvert.DeserializeObject<ComponentSubmitRequest>(Encoding.ASCII.GetString(e.MessageBody));
                    this.SaveComponentData(submitReq, (IPEndPoint)e.Server.Client.RemoteEndPoint);
                    this.SendComponentSubmitRespose(e.Server, submitReq.ComponentSubmitRequestGuid);
                    break;
                case Core.Network.MessageCode.JobRequest:
                    JobRequest jobReq = JsonConvert.DeserializeObject<JobRequest>(Encoding.ASCII.GetString(e.MessageBody));
                    this.SendJobResponse(e.Server, jobReq.JobRequestGuid, true);
                    this.TryToExecuteJob(jobReq, e.Server);
                    break;
                case Core.Network.MessageCode.RequestAssembly:
                    AssemblyRequest assemblyRequest = JsonConvert.DeserializeObject<AssemblyRequest>(Encoding.ASCII.GetString(e.MessageBody));
                    this.SendAssembly(e.Server, assemblyRequest.ComponentGuid);
                    break;
                case Core.Network.MessageCode.ClientUpdate:
                    ClientUpdateRequest clientUpdReq = JsonConvert.DeserializeObject<ClientUpdateRequest>(Encoding.ASCII.GetString(e.MessageBody));
                    this.SaveClientUpdate(clientUpdReq, (IPEndPoint)e.Server.Client.RemoteEndPoint);
                    this.SendClientUpdateResponse(e.Server, clientUpdReq.ClientUpdateRequestGuid);
                    break;
                default:
                    break;
            }
        }

        private void SendJobResponse(TcpClient client, Guid guid, bool p)
        {
            JobResponse resp = new JobResponse();
            resp.IsAccepted = p;
            resp.JobRequestGuid = guid;

            string json = JsonConvert.SerializeObject(resp);
            var bytes = DataConverter.ConvertJsonToByteArray(MessageCode.JobRequest, json);

            try
            {
                NetworkStream ns = client.GetStream();

                ns.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void TryToExecuteJob(JobRequest jobReq, TcpClient client)
        {
            // TODO:
            //var results = SplitJob.Split(jobReq, this.TcpManager);
            List<object> results = null;

            JobResultRequest req = new JobResultRequest();
            req.JobGuid = jobReq.JobGuid;
            req.JobResultGuid = Guid.NewGuid();
            req.OutputData = results;
            req.State = JobState.Ok;

            string json = JsonConvert.SerializeObject(req);
            var bytes = DataConverter.ConvertJsonToByteArray(MessageCode.JobResult, json);

            try
            {
                NetworkStream ns = client.GetStream();

                ns.Write(bytes, 0, bytes.Length);

                // wait for response result

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void SaveClientUpdate(ClientUpdateRequest clientUpdReq, IPEndPoint iPEndPoint)
        {
            var serverguid = this.Server.Servers.FirstOrDefault(x => x.Value.Address == iPEndPoint.Address && x.Value.Port == iPEndPoint.Port).Key;

            if (clientUpdReq.ClientState == ClientState.Connected)
            {
                this.TcpManager.AllServerClients.FirstOrDefault(x => x.Key == serverguid).Value.Add(clientUpdReq.ClientInfo);
            }
            else if (clientUpdReq.ClientState == ClientState.Disconnected)
            {
                var listOfClients = this.TcpManager.AllServerClients.FirstOrDefault(x => x.Key == serverguid).Value;
                foreach (var item in listOfClients)
                {
                    if (item.ClientGuid == clientUpdReq.ClientInfo.ClientGuid)
                    {
                        listOfClients.Remove(item);
                        break;
                    }
                }
            }
        }

        private void SendClientUpdateResponse(TcpClient tcpClient, Guid guid)
        {
            ClientUpdateResponse resp = new ClientUpdateResponse();
            resp.ClientUpdateRequestGuid = guid;

            string json = JsonConvert.SerializeObject(resp);
            var bytes = DataConverter.ConvertJsonToByteArray(MessageCode.ClientUpdate, json);

            try
            {
                NetworkStream ns = tcpClient.GetStream();

                ns.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                tcpClient.Close();
            }
        }

        private void SendAssembly(TcpClient tcpClient, Guid guid)
        {
            var component = this.TcpManager.Components.FirstOrDefault(x => x.Key == guid).Value;
            string path = this.TcpManager.Dlls[guid];
            byte[] data = DataConverter.ConvertDllToByteArray(path);
            byte[] message = new byte[data.Length + 5];
            message[0] = (byte)MessageCode.RequestAssembly;

            var length = BitConverter.GetBytes(data.Length);
            for (int i = 1; i < 5; i++)
            {
                message[i] = length[i - 1];
            }

            for (int i = 5; i < message.Length; i++)
            {
                message[i] = data[i];
            }

            try
            {
                NetworkStream ns = tcpClient.GetStream();

                ns.Write(message, 0, message.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                tcpClient.Close();
            }
        }

        private void SendComponentSubmitRespose(TcpClient tcpClient, Guid guid)
        {
            ComponentSubmitResponse resp = new ComponentSubmitResponse();
            resp.ComponentSubmitRequestGuid = guid;
            resp.IsAccepted = true;

            string json = JsonConvert.SerializeObject(resp);
            var bytes = DataConverter.ConvertJsonToByteArray(MessageCode.ComponentSubmit, json);

            try
            {
                NetworkStream ns = tcpClient.GetStream();

                ns.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                tcpClient.Close();
            }
        }

        private void SaveComponentData(ComponentSubmitRequest submitResp, IPEndPoint ipEndPoint)
        {
            var guid = this.Server.Servers.FirstOrDefault(x => x.Value == ipEndPoint).Key;
            this.TcpManager.AllServerComponents.FirstOrDefault(x => x.Key == guid).Value.Add(submitResp.Component);
        }

        private void SaveKeepAliveData(KeepAliveRequest keepAliveReq)
        {
            if (keepAliveReq.Terminate)
            {
                this.Server.Servers.Remove(keepAliveReq.ServerGuid);
            }
            else
            {
                this.TcpManager.AllServerCpuLoads.Add(keepAliveReq.ServerGuid, keepAliveReq.CpuLoad);
            }
        }

        private void SendKeepAliveResponse(TcpClient tcpClient, Guid guid)
        {
            KeepAliveResponse resp = new KeepAliveResponse();
            resp.KeepAliveRequestGuid = guid;

            string json = JsonConvert.SerializeObject(resp);
            var bytes = DataConverter.ConvertJsonToByteArray(MessageCode.KeepAlive, json);

            try
            {
                NetworkStream ns = tcpClient.GetStream();
                ns.Write(bytes, 0, bytes.Length);
            }
            catch
            {
            }
            finally
            {
                tcpClient.Close();
            }
        }

        private void SendLogonResponse(TcpClient server, Guid logonRequestGuid)
        {
            LogonResponse resp = new LogonResponse();
            resp.LogonRequestGuid = logonRequestGuid;
            resp.ServerGuid = this.Server.ServerGuid;
            resp.FriendlyName = this.Server.FriendlyName;
            resp.AvailableClients = this.TcpManager.MyTCPServer.Clients.Keys;
            resp.AvailableComponents = this.TcpManager.Components.Values;

            string json = JsonConvert.SerializeObject(resp);
            var bytes = DataConverter.ConvertJsonToByteArray(MessageCode.Logon, json);

            try
            {
                NetworkStream ns = server.GetStream();

                ns.Write(bytes, 0, bytes.Length);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            server.Close();
        }

        internal void SendLogonRequest(IPEndPoint groupEP)
        {
            groupEP.Port = 8080;

            TcpClient client = new TcpClient();
            client.Connect(groupEP);

            LogonRequest req = new LogonRequest();
            req.AvailableClients = this.TcpManager.MyTCPServer.Clients.Keys;
            req.AvailableComponents = this.TcpManager.Components.Values;
            req.FriendlyName = this.Server.FriendlyName;
            req.LogonRequestGuid = Guid.NewGuid();
            req.ServerGuid = this.Server.ServerGuid;

            string json = JsonConvert.SerializeObject(req);
            var bytes = DataConverter.ConvertJsonToByteArray(MessageCode.Logon, json);

            try
            {
                NetworkStream ns = client.GetStream();

                ns.Write(bytes, 0, bytes.Length);

                bool run = true;

                while (run)
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

                                for (int i = 0; i < 5; i++)
                                {
                                    messageLength[i] = buffer[1 + i];
                                }

                                var length = BitConverter.ToInt32(messageLength, 0) + 5;
                                messagetype = buffer[0];
                                body = new byte[length - 5];

                                for (int i = 5; i < recievedbytes; i++)
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
                                    if ((int)messagetype == (int)MessageCode.Logon)
                                    {
                                        if (this.SaveServerEndpoint(body, groupEP, req.LogonRequestGuid))
                                        {
                                            run = false;
                                        }
                                    }
                                }
                            }

                            for (int i = 0; i < recievedbytes; i++)
                            {
                                body[index] = buffer[i];
                                index++;
                            }

                            if (index >= body.Length)
                            {
                                if ((int)messagetype == (int)MessageCode.Logon)
                                {
                                    if (this.SaveServerEndpoint(body, groupEP, req.LogonRequestGuid))
                                    {
                                        run = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            client.Close();
        }

        private bool SaveServerEndpoint(byte[] body, IPEndPoint client, Guid logonRequestGuid)
        {
            string json = Encoding.ASCII.GetString(body);
            LogonResponse resp = JsonConvert.DeserializeObject<LogonResponse>(json);
            if (resp.LogonRequestGuid == logonRequestGuid)
            {
                this.Server.Servers.Add(resp.ServerGuid, client);
                this.TcpManager.AllServerComponents.Add(resp.ServerGuid, resp.AvailableComponents.ToList());
                this.TcpManager.AllServerClients.Add(resp.ServerGuid, resp.AvailableClients.ToList());

                return true;
            }

            return false;
        }

        internal int CalcCPULoad()
        {
            throw new NotImplementedException();
        }

        internal int GetNumberOfClients()
        {
            throw new NotImplementedException();
        }
    }
}
