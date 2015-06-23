namespace Server
{
    using Core.Network;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ClientFetchedEventArgs : EventArgs
    {
        public ClientFetchedEventArgs(ClientInfo info)
        {
            this.ClientInfo = info;
        }

        public ClientInfo ClientInfo { get; set; }
    }
}
