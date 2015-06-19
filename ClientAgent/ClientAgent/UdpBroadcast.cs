
namespace ClientAgent
{
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Provides a method for boadcsting messages.
    /// </summary>
    public static class UdpBroadcast
    {
        /// <summary>
        /// Sends
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

            cl.Close();
        }

    }
}
