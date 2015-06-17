using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientAgent
{
    public class ClientListenerThreadArgs : ThreadArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientListenerThreadArgs"/> class.
        /// </summary>
        /// <param name="c">
        ///     The client used for the connection.
        /// </param>
        public ClientListenerThreadArgs(Client c)
            : base()
        {

        }

        /// <summary>
        /// Gets the <see cref="Client"/> instance used by the thread.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="Client"/> instance used by the thread.
        /// </value>
        public Client Client
        {
            get;
            private set;
        }
    }
}
