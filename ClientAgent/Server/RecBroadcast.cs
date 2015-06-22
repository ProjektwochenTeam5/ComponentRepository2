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

        public void Recieve()
        {

            bool done = false;
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, this.UdpClientPort);
            UdpClient client = new UdpClient(groupEP);
            client.EnableBroadcast = true;
            Console.WriteLine("--> UDP waiting for Broadcast!");

            try
            {
                while (!done)
                {
                    byte[] bytes = client.Receive(ref groupEP);

                    ////////////// For testing purposes
                    Console.WriteLine("Recieved broadcast from {0} :\n {1}\n", groupEP.ToString(), Encoding.ASCII.GetString(bytes, 0, bytes.Length));


                    //// TODO: Check if Messagetype == 1
                    //////// Send IP Back - store client
                    groupEP.Port = 1234;
                    this.SendIP(groupEP, client);
                }
            }
            catch (Exception e)
            {
                //////////// for testing

                Console.WriteLine(e.Message);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string GetMyIP()
        {

            string HostName = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry hostInfo = System.Net.Dns.GetHostByName(HostName);
            string IpAdresse = hostInfo.AddressList[0].ToString();

            return IpAdresse;
        }
    }
}
