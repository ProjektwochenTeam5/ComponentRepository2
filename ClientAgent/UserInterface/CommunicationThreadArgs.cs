using System;
using System.Net.Sockets;

namespace UserInterface
{
    public class CommunicationThreadArgs
    {
        public CommunicationThreadArgs(TcpClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            this.Client = client;
        }

        public TcpClient Client
        {
            get;
            private set;
        }
    }
}
