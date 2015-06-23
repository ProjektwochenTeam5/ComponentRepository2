
namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// Provides a method for boadcsting messages.
    /// </summary>
    public static class UdpBroadcast
    {
        /// <summary>
        /// Sends a broadcast.
        /// </summary>
        /// <param name="port">
        ///     
        /// </param>
        /// <param name="data">
        ///     
        /// </param>
        public static void SendBoadcast(int port, byte[] data)
        {
            ////byte current = 1;
            UdpClient cl = new UdpClient();
            cl.EnableBroadcast = true;
            cl.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, port));
            cl.Close();
        }
    }
}
