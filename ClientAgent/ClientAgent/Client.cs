using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientAgent
{
    public class Client
    {
        private Thread listenerThread;


        /// <summary>
        /// 
        /// </summary>
        public Client()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public TcpClient Listener
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendVisible()
        {
            UdpClient cl = new UdpClient(new IPEndPoint(IPAddress.Any, 1234));
            List<byte> msg = new List<byte>();
            msg.AddRange(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 });

            cl.Send(msg.ToArray(), msg.Count, new IPEndPoint(IPAddress.Broadcast, 1234));
        }


        private static void ListenerThread(object data)
        {

        }
    }
}
