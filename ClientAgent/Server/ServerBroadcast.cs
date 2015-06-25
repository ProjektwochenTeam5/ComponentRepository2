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
    public class ServerBroadcast
    {
        public UdpClient Client { get; set; }

        public ServerBroadcast()
        {
            this.Client = new UdpClient();
        }

        ~ServerBroadcast()
        {
            this.Client.Close();
        }

        public void SendBroadcast()
        {
            while (true)
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 8081);
                byte[] bytes = Encoding.ASCII.GetBytes("PWSP");
                this.Client.Send(bytes, bytes.Length, ip);
                Console.WriteLine("Broadcast sent.");
                Thread.Sleep(180000);
            }
        }
    }
}
