using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class AgentDiscover : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentDiscover"/> class.
        /// </summary>
        public AgentDiscover()
        {
            this.FriendlyName = Environment.MachineName;
        }

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


        /// <summary>
        /// Gets or sets the FriendlyName of the agent.
        /// </summary>
        /// <value>
        ///     Contains the friendly name of the agent.
        /// </value>
        public string FriendlyName { get; set; }


    }
}
