﻿
namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

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
            Task t150 = new Task(delegate()
                {
                    UdpClient cl = new UdpClient();
                    cl.EnableBroadcast = true;
                    ////cl.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, port));
            
                    for (byte n = 1; n < 0xff; n++)
                    {
                        cl.Send(data, data.Length, new IPEndPoint(new IPAddress(new byte[] { 10, 101, 150, n }), port));
                    }

                    cl.Close();
                });

            t150.Start();

            Task t100 = new Task(delegate()
            {
                UdpClient cl = new UdpClient();
                cl.EnableBroadcast = true;
                ////cl.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, port));

                for (byte n = 1; n < 0xff; n++)
                {
                    cl.Send(data, data.Length, new IPEndPoint(new IPAddress(new byte[] { 10, 101, 100, n }), port));
                }

                cl.Close();
            });

            t100.Start();
        }
    }
}
