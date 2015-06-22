// --------------------------------------------------------------
// <copyright file="ClientListenerThreadArgs.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="ClientListenerThreadArgs"/> class.
// </summary>
// <author>
// David Eiwen
// </author>
// --------------------------------------------------------------

namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a way to communicate with a thread communicating with a server.
    /// </summary>
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
            this.Client = c;
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
