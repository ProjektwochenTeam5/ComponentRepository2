using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientAgent
{
    public static class UdpBroadcast
    {
        public static void SendBoadcast(int port, byte[] data)
        {
            byte current = 1;
            UdpClient cl = new UdpClient();

            for(; current < 0xff; current++)
            {
                IPEndPoint target = new IPEndPoint(new IPAddress(new byte[] { 10, 101, 150, current }), port);
                cl.Send(data, data.Length, target);
            }

            cl.Close();
        }

    }
}
