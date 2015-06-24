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
    public class JobServerToServer
    {
        public JobServerToServer(TcpServerToServerManager manager)
        {
            this.Manager = manager;
        }

        public TcpServerToServerManager Manager { get; set; }

        public List<object> SendJobRequest(IPEndPoint ipEndPoint, Component component, Guid? targetDisplay, Guid? targetCalc, List<object> inputData, string friendly)
        {
            List<object> results = null;
            TcpClient client = new TcpClient();

            try
            {
                JobRequest req = new JobRequest();
                req.JobGuid = Guid.NewGuid();
                req.JobRequestGuid = Guid.NewGuid();
                req.TargetCalcClientGuid = targetCalc;
                req.TargetDisplayClient = targetDisplay;
                req.HopCount = 0;
                req.JobSourceClientGuid = this.Manager.Server.ServerGuid;
                req.JobComponent = component;
                req.InputData = inputData;
                req.FriendlyName = friendly;

                client.Connect(ipEndPoint);

                string json = JsonConvert.SerializeObject(req);
                var bytes = DataConverter.ConvertJsonToByteArray(MessageCode.JobRequest, json);

                int waitForResponse = 0;

                NetworkStream ns = client.GetStream();

                ns.Write(bytes, 0, bytes.Length);

                bool run = true;

                while (run)
                {
                    if (waitForResponse > 120)
                    {
                        this.Manager.Server.Servers.Remove(this.Manager.Server.Servers.FirstOrDefault(x => x.Value.Address == ipEndPoint.Address && x.Value.Port == ipEndPoint.Port).Key);
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
                                    if ((int)messagetype == (int)MessageCode.JobRequest)
                                    {
                                        if (this.CheckJobResponse(req.JobRequestGuid, body))
                                        {
                                            run = false;

                                            // wait for job result request
                                            results = this.GetJobResult(req.JobGuid, client);
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
                                if ((int)messagetype == (int)MessageCode.JobRequest)
                                {
                                    if (this.CheckJobResponse(req.JobRequestGuid, body))
                                    {
                                        run = false;

                                        // wait for job result request
                                        results = this.GetJobResult(req.JobGuid, client);
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

            return results;
        }

        private List<object> GetJobResult(Guid guid, TcpClient client)
        {
            List<object> results = null;
            try
            {
                int waitForResponse = 0;
                NetworkStream ns = client.GetStream();
                bool run = true;

                while (run)
                {
                    if (waitForResponse > 300)
                    {
                        this.Manager.Server.Servers.Remove
                            (this.Manager.Server.Servers.FirstOrDefault
                            (x => x.Value.Address == ((IPEndPoint)client.Client.RemoteEndPoint).Address && 
                                x.Value.Port == ((IPEndPoint)client.Client.RemoteEndPoint).Port).Key);
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
                                    if ((int)messagetype == (int)MessageCode.JobResult)
                                    {
                                        return this.CheckJobResultRequest(body, guid, client);
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
                                if ((int)messagetype == (int)MessageCode.JobResult)
                                {
                                    return this.CheckJobResultRequest(body, guid, client);
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

            return results;
        }

        private List<object> CheckJobResultRequest(byte[] body, Guid guid, TcpClient tcpClient)
        {
            string json = Encoding.ASCII.GetString(body);
            JobResultRequest req = JsonConvert.DeserializeObject<JobResultRequest>(json);
            if (req.JobGuid == guid)
            {
                if (req.State != JobState.Ok)
                {
                    return null;
                }
                else
                {
                    this.SendJobResultResponse(req.JobResultGuid, tcpClient);
                    return req.OutputData.ToList();
                }
            }
            else
            {
                return null;
            }
        }

        private void SendJobResultResponse(Guid guid, TcpClient tcpClient)
        {
            JobResultResponse resp = new JobResultResponse();
            resp.JobResultGuid = guid;

            string json = JsonConvert.SerializeObject(resp);
            var bytes = DataConverter.ConvertJsonToByteArray(MessageCode.JobResult, json);

            try
            {
                NetworkStream ns = tcpClient.GetStream();
                ns.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool CheckJobResponse(Guid guid, byte[] body)
        {
            string json = Encoding.ASCII.GetString(body);
            JobResponse resp = JsonConvert.DeserializeObject<JobResponse>(json);
            return (resp.IsAccepted && resp.JobRequestGuid == guid);
        }
    }
}
