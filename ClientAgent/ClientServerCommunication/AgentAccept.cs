using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class AgentAccept : Message
    {
        public IPEndPoint ServerIP { get; set; }
    }
}
