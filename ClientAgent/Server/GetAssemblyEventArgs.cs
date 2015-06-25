using Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class GetAssemblyEventArgs : EventArgs
    {
        public GetAssemblyEventArgs(ClientInfo info, Guid messageID, Guid componentID)
        {
            this.Info = info;
            this.MessageID = messageID;
            this.ComponentID = componentID;
        }

        public ClientInfo Info { get; set; }

        public Guid MessageID { get; set; }

        public Guid ComponentID { get; set; }
    }
}
