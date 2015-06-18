namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ClientFetchedEventArgs : EventArgs
    {
        public ClientFetchedEventArgs(Guid clientid)
        {
            this.ClientId = clientid;
        }

        public Guid ClientId { get; set; }
    }
}
