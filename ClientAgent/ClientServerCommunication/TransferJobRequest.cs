﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class TransferJobRequest : Message
    {
        public DoJobRequest Job { get; set; }

        public Guid ServerID { get; set; }

        public Guid ClientID { get; set; }

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
