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

        /// <summary>
        /// Gets the message type of the message.
        /// </summary>
        /// <value>
        ///     Contains the message type of the message.
        /// </value>
        public override StatusCode MessageType
        {
            get { return StatusCode.AgentConnection; }
        }
    }
}
