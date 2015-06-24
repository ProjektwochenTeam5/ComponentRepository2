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
                    JobResponse jobResp = JsonConvert.DeserializeObject<JobResponse>(Encoding.ASCII.GetString(e.MessageBody));

                    break;
                case Core.Network.MessageCode.JobResult:
                    JobResultRequest jobResultReq = JsonConvert.DeserializeObject<JobResultRequest>(Encoding.ASCII.GetString(e.MessageBody));

                    break;
                case Core.Network.MessageCode.RequestAssembly:
                    
                    break;
                case Core.Network.MessageCode.ClientUpdate:
                    ClientUpdateResponse clientUpdResp = JsonConvert.DeserializeObject<ClientUpdateResponse>(Encoding.ASCII.GetString(e.MessageBody));
                    break;
                default:
                    break;
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
