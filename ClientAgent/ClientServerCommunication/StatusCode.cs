using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    public enum StatusCode
    {
        KeepAlive = 0,

        AgentConnection = 1,

        Acknowledge = 3,

        TransferComponent = 4,

        TransferJob = 5,

        SendComponentInfos = 6,

        DoJobRequest = 7,

        StorComponent = 8,

        Error = 9
    }
}
