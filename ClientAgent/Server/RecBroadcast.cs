using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class RecBroadcast
    {
        public void Recieve()
        {
            while (true)
            {
                Socket sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

                IPEndPoint iep = new IPEndPoint(IPAddress.Any, 1234);
                sock.Bind(iep);

                EndPoint ep = (EndPoint)iep;
                Console.WriteLine("Ready to receive...");

                byte[] data = new byte[1024];
                int recv = sock.ReceiveFrom(data, ref ep);
                string stringData = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine("received: {0}  from: {1}",
                                      stringData, ep.ToString());

                data = new byte[1024];
                recv = sock.ReceiveFrom(data, ref ep);
                stringData = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine("received: {0}  from: {1}",
                                      stringData, ep.ToString());
                sock.Close();
            }
        }
    }
}
