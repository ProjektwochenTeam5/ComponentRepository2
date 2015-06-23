using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable()]
    public class ExecuteRequest : Message
    {
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

        /// <summary>
        ///  Gets or sets the Assembly to execute.
        /// </summary>
        /// <value>
        ///     The byte representation of the file.
        /// </value>
        public byte[] Assembly { get; set; }

        /// <summary>
        /// Gets or sets the input-data for executing.
        /// </summary>
        /// <value>
        ///     The data needed for executing.
        /// </value>
        public object[] InputData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteRequest"/> class.
        /// </summary>
        public ExecuteRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteRequest"/> class.
        /// </summary>
        /// <param name="assembly">The assembly to execute.</param>
        /// <param name="inputData">The data needed for executing.</param>
        public ExecuteRequest(byte[] assembly, object[] inputData)
        {
            this.Assembly = assembly;
            this.InputData = inputData;
        }
    }
}
