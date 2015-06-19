namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public static class Program
    {
        static void Main(string[] args)
        {
            TcpClient c = new TcpClient(new IPEndPoint(IPAddress.Any, 12345));
            Client cl = new Client(c);
            ClientMessageManager mgr = new ClientMessageManager(cl);
            cl.Connected += cl_Connected;
            cl.ReceivedTCPMessage += cl_ReceivedTCPMessage;
            cl.StartConnectionSearch();
            Console.WriteLine("Started Connection Search!\nPress [ENTER] to stop...");
            Console.ReadLine();

            cl.StopAll();

            Console.WriteLine("Stopped!");
            Console.ReadLine();
        }

        static void cl_ReceivedTCPMessage(object sender, MessageReceivedEventArgs e)
        {
        }

        static void cl_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected to server!");
        }
    }
}
