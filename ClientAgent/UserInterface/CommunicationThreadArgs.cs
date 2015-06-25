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
            this.Exit = false;
        }

        public bool Exit
        {
            get;
            set;
        }

        public TcpClient Client
        {
            get;
            private set;
        }
    }
}
