using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientAgent
{
    /// <summary>
    /// 
    /// </summary>
    public class Client
    {
        /// <summary>
        /// 
        /// </summary>
        private Thread listenerThread;

        /// <summary>
        /// 
        /// </summary>
        private Thread udpListenerThread;

        /// <summary>
        /// 
        /// </summary>
        private ClientListenerThreadArgs listenerThreadArgs;

        /// <summary>
        /// 
        /// </summary>
        private UdpClientThreadArgs udpListenerThreadArgs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="cl">
        ///     The <see cref="TcpClient"/> instance that is used by the client.
        /// </param>
        public Client(TcpClient cl)
        {
            this.ConnectionClient = cl;
        }

        /// <summary>
        /// Gets the <see cref="System.Net.Sockets.TcpClient"/> instance linked to the client.
        /// </summary>
        public TcpClient ConnectionClient
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">
        ///     The arguments passed to the thread.
        /// </param>
        private static void UdpThread(object data)
        {
            UdpClientThreadArgs args = (UdpClientThreadArgs)data;

            while (!args.Stopped)
            {
                if (args.UdpClient.Available > 0)
                {
                    byte[] header = new byte[4];
                    args.UdpClient.Client.Receive(header);
                    if (header[0] == 0 && header[1] == 0 && header[2] == 0 && header[3] == 0)
                    {

                    }
                }

                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">
        ///     The arguments passed to the thread.
        /// </param>
        private static void ListenerThread(object data)
        {
            ClientListenerThreadArgs args = (ClientListenerThreadArgs)data;

            while (!args.Stopped)
            {


                Thread.Sleep(5);
            }
        }
    }
}
