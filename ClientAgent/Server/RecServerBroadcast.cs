using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class RecServerBroadcast
    {
        public RecServerBroadcast(TcpServerToServerManager manager)
        {
            this.Manager = manager;
        }

        public TcpServerToServerManager Manager { get; set; }

        public int UdpClientPort { get { return 8081; } }

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

                    if (bytes == Encoding.ASCII.GetBytes("PWSP"))
                    {
                        if (this.Manager.Server.Servers.Values.FirstOrDefault(x => x.Address == groupEP.Address && x.Port == groupEP.Port) == null)
                        {
                            this.Manager.SendLogonRequest(groupEP);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}
