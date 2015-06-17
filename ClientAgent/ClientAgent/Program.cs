namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            TcpClient c = new TcpClient(new IPEndPoint(IPAddress.Any, 12345));
            Client cl = new Client(c);
            cl.StartConnectionSearch();
            Console.WriteLine("Started Connection Search!");
            Console.ReadLine();
        }
    }
}
