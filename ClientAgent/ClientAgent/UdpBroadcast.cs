
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
            byte current = 1;
            UdpClient cl = new UdpClient();

            for(; current < 0xff; current++)
            {
                IPEndPoint target = new IPEndPoint(new IPAddress(new byte[] { 10, 101, 150, current }), port);
                cl.Send(data, data.Length, target);
            }

            /*foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var i in ni.GetIPProperties().UnicastAddresses)
                {
                    if (i.IPv4Mask == IPAddress.Any || IPAddress.IsLoopback(i.Address))
                    {
                        continue;
                    }

                    Thread t = new Thread(delegate(object dat)
                    {
                        object[] o = (object[])dat;

                        UnicastIPAddressInformation inf = (UnicastIPAddressInformation)o[0];

                        ////i.PrefixLength;

                        byte[] b = inf.IPv4Mask.GetAddressBytes();

                        uint addr = (uint)(b[3] + 0x100 * b[2] + 0x10000 * b[1] + 0x1000000 * b[0]);

                        byte[] bits = new byte[32];
                        uint address = 0;

                        for (int n = 0; n < 32; n++)
                        {
                            bits[n] = (byte)(addr / Math.Pow(2, n) % 2);
                        }

                        Console.WriteLine("{0} - {1}", string.Join(string.Empty, bits.Reverse()), i.Address);
                    });

                    t.Start(new object[] { i });
                    ////Console.WriteLine("{0} - {1} Network Bits: {2} - Host Bits: {3}", i.Address, i.IPv4Mask, i.PrefixLength, 32 - i.PrefixLength);
                }
            }*/

            cl.Close();
        }
    }
}
