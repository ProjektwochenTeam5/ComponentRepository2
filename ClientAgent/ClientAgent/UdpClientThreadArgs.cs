// --------------------------------------------------------------
// <copyright file="UdpClientThreadArgs.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="UdpClientThreadArgs"/> class.
// </summary>
// <author>
// David Eiwen
// </author>
// --------------------------------------------------------------

namespace ClientAgent
{
    using System.Net.Sockets;

    /// <summary>
    /// Provides a way to communicate with a UDP listener thread.
    /// </summary>
    public class UdpClientThreadArgs
        : ClientListenerThreadArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClientThreadArgs"/> class.
        /// </summary>
        /// <param name="cl">
        ///     The <see cref="System.Net.Sockets.UdpClient"/> instance used by the thread. 
        /// </param>
        /// <param name="c">
        ///     The <see cref="Client"/> instance used by the thread.
        /// </param>
        public UdpClientThreadArgs(UdpClient cl, Client c)
            : base(c)
        {
            this.UdpClient = cl;
        }

        /// <summary>
        /// Gets the <see cref="System.Net.Sockets.UdpClient"/> instance used by the thread.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="System.Net.Sockets.UdpClient"/> instance used by the thread.
        /// </value>
        public UdpClient UdpClient
        {
            get;
            private set;
        }
    }
}
