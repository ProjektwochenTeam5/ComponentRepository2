using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class KeepAlive : Message
    {
        /// <summary>
        /// Gets or sets the CPU load in percent.
        /// </summary>
        public int CPUWorkload { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client shall terminate.
        /// </summary>
        public bool Terminate { get; set; }

        /// <summary>
        /// Gets the message type of the message.
        /// </summary>
        /// <value>
        ///     Contains the message type of the message.
        /// </value>
        public override StatusCode MessageType
        {
            get { return StatusCode.KeepAlive; }
        }
    }
}
