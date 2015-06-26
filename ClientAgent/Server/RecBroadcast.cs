using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommunication;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    public class RecBroadcast
    {
        public int UdpClientPort { get { return 1233; } }

        public string GetTime { get { return DateTime.Now.ToLongTimeString() + " --> "; } }


        public event EventHandler<UdpClientDiscoverRecievedEventArgs> OnUdpClientDiscovered;

        public void Recieve()
        {

            bool done = false;
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, this.UdpClientPort);
            UdpClient client = new UdpClient(groupEP);
            client.EnableBroadcast = true;
            Console.WriteLine(this.GetTime + "UDP waiting for Broadcast!");

            try
            {
                while (!done)
                {
                    if (client.Available > 0)
                    {
                        byte[] bytes = client.Receive(ref groupEP);

                        if (bytes[8] == (byte)1)
                        {
                            byte[] body = bytes.SubArray(9, bytes.Length - 9);
                            Console.WriteLine(this.GetTime + "Recieved broadcast from {0}", groupEP.ToString());

                            AgentDiscover discover = (AgentDiscover)DataConverter.ConvertByteArrayToMessage(body);
                            if (discover != null)
                            {
                                this.FireOnClientDiscovered(new UdpClientDiscoverRecievedEventArgs(groupEP.Address.ToString(), discover.FriendlyName));
                            }
                        }

                        groupEP.Port = 1234;
                        this.SendIP(groupEP, client);
                    }
                }
            }
            catch (Exception e)
            {
                //////////// for testing

                Console.WriteLine(this.GetTime + e.Message);
            }
            finally
            {
                client.Close();
            }

        }

        public void SendIP(IPEndPoint ipendpoint, UdpClient client)
        {
            AgentAccept a = new AgentAccept();
            IPAddress myip = IPAddress.Parse(this.GetMyIP());

            IPEndPoint p = new IPEndPoint(myip, 12345);
            a.ServerIP = p;
            
            var send = DataConverter.ConvertMessageToByteArray(1, DataConverter.ConvertObjectToByteArray(a));

            try
            {
                client.Send(send, send.Length, ipendpoint);
                Console.WriteLine(this.GetTime + "Sending ip + {0}", send.Length + "bytes");
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.GetTime + ex.Message);
            }
        }

        public string GetMyIP()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostInfo = Dns.GetHostByName(hostName);
            string ipAdress = hostInfo.AddressList[0].ToString();

            return ipAdress;
        }

        protected void FireOnClientDiscovered(UdpClientDiscoverRecievedEventArgs e)
        {
            if (this.OnUdpClientDiscovered != null)
            {
                this.OnUdpClientDiscovered(this, e);
            }
        }
    }
}
