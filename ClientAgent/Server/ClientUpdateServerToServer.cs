using ClientServerCommunication;
using Core.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ClientUpdateServerToServer
    {
        public ClientUpdateServerToServer(TcpServerToServerManager manager)
        {
            this.Manager = manager;
        }

        public TcpServerToServerManager Manager { get; set; }

        public void SendClientUpdateRequest(Dictionary<Guid, IPEndPoint> servers, ClientInfo clientInfo, ClientState state)
        {
            foreach (var ipEndPoint in servers.Values)
            {
                TcpClient client = new TcpClient();

                try
                {
                    client.Connect(ipEndPoint);

                    ClientUpdateRequest req = new ClientUpdateRequest();
                    req.ClientUpdateRequestGuid = Guid.NewGuid();
                    req.ClientInfo = clientInfo;
                    req.ClientState = state;

                    string json = JsonConvert.SerializeObject(req);
                    var bytes = DataConverter.ConvertJsonToByteArray(MessageCode.ClientUpdate, json);

                    int waitForResponse = 0;

                    NetworkStream ns = client.GetStream();

                    ns.Write(bytes, 0, bytes.Length);

                    bool run = true;

                    while (run)
                    {
                        if (waitForResponse > 120)
                        {
                            this.Manager.Server.Servers.Remove(servers.FirstOrDefault(x => x.Value == ipEndPoint).Key);
                            break;
                        }

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
                                        if ((int)messagetype == (int)MessageCode.ClientUpdate)
                                        {
                                            if (this.CheckClientResponse(req.ClientUpdateRequestGuid, body))
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
                                    if ((int)messagetype == (int)MessageCode.ComponentSubmit)
                                    {
                                        if (this.CheckClientResponse(req.ClientUpdateRequestGuid, body))
                                        {
                                            run = false;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            waitForResponse++;
                        }
                    }
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
        }

        private bool CheckClientResponse(Guid guid, byte[] body)
        {
            string json = Encoding.ASCII.GetString(body);
            ClientUpdateResponse resp = JsonConvert.DeserializeObject<ClientUpdateResponse>(json);
            return (resp.ClientUpdateRequestGuid == guid);
        }
    }
}
