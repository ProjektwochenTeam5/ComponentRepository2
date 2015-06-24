using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class StoreComponent : Message
    {
        public byte[] Component { get; set; }

        public bool IsComplex { get; set; }

        /// <summary>
        /// The friendly name of the component to store.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets the message type of the message.
        /// </summary>
        /// <value>
        ///     Contains the message type of the message.
        /// </value>
        public override StatusCode MessageType
        {
            get { return StatusCode.StorComponent; }
        }
    }
}
