using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable()]
    public class ExecuteResponse : Message
    {
        public object[] Result { get; set; }

        public bool Error { get; set; }

        public ExecuteResponse() : base()
        {
        }

        public ExecuteResponse(object[] result, bool error) : base()
        {
            this.Result = result;
            this.Error = error;
        }

        /// <summary>
        /// Gets the message type of the message.
        /// </summary>
        /// <value>
        ///     Contains the message type of the message.
        /// </value>
        public override StatusCode MessageType
        {
            get { return StatusCode.ExecuteJob; }
        }
    }
}
