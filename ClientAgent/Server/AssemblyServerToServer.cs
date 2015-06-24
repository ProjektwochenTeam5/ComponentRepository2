using ClientServerCommunication;
using Core.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class AssemblyServerToServer
    {
        public AssemblyServerToServer(TcpServerToServerManager manager)
        {
            this.Manager = manager;
        }

        public TcpServerToServerManager Manager { get; set; }

        public byte[] SendAssemblyRequest(Dictionary<Guid, IPEndPoint> servers, Guid componentGuid)
        {
            var components = this.Manager.TcpManager.AllServerComponents;

            Guid targetServer = new Guid();

            foreach (var component in components)
            {
                foreach (var item in component.Value)
                {
                    if (item.ComponentGuid == componentGuid)
                    {
                        targetServer = component.Key;
                        break;
                    }
                }
            }

            var ipEndPoint = servers.FirstOrDefault(x => x.Key == targetServer);

            if (targetServer != default(Guid))
            {
                TcpClient client = new TcpClient();
                try
                {
                    client.Connect(servers.FirstOrDefault(x => x.Key == targetServer).Value);

                    AssemblyRequest req = new AssemblyRequest();
                    req.AssemblyRequestGuid = Guid.NewGuid();
                    req.ComponentGuid = componentGuid;

                    string json = JsonConvert.SerializeObject(req);
                    var bytes = DataConverter.ConvertJsonToByteArray(MessageCode.RequestAssembly, json);

                    int waitForResponse = 0;
                    NetworkStream ns = client.GetStream();

                    ns.Write(bytes, 0, bytes.Length);

                    bool run = true;

                    while (run)
                    {
                        if (waitForResponse > 120)
                        {
                            this.Manager.Server.Servers.Remove(targetServer);
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
                                        if ((int)messagetype == (int)MessageCode.RequestAssembly)
                                        {
                                            return body;
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
                                    if ((int)messagetype == (int)MessageCode.RequestAssembly)
                                    {
                                        return body;
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
            else
            {
                this.Manager.TcpManager.AllServerComponents.Remove(componentGuid);
            }

            return null;
        }
    }
}
