using Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class TransferJobRequest : Message
    {
        public Guid ComponentGuid { get; set; }

        public Guid ServerID { get; set; }
        
        /// <summary>
        /// Gets or sets a collection of input arguments.
        /// </summary>
        /// <value>Collection of objects.</value>
        public IEnumerable<object> InputData { get; set; }

        /// <summary>
        /// Gets the message type of the message.
        /// </summary>
        /// <value>
        ///     Contains the message type of the message.
        /// </value>
        public override StatusCode MessageType
        {
            get { return StatusCode.TransferJob; }
        }
    }
}
