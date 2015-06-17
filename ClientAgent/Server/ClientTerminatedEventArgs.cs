using Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientTerminatedEventArgs
    {
        public ClientInfo ClientWhoWantsToQuit { get; set; }

        public ClientTerminatedEventArgs(ClientInfo i)
        {
            this.ClientWhoWantsToQuit = i;
        }
    }
}
