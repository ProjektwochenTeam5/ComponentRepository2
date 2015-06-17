// ----------------------------------------------------------------------- 
// <copyright file="KeepAliveRequest.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the KeepAliveRequest class.</summary> 
// <author>Michael Sabransky</author> 
// -----------------------------------------------------------------------
namespace Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Send frequently to all other servers to update them on the server state.
    /// </summary>
    public class KeepAliveRequest 
    {
        /// <summary>
        /// Gets or sets the unique id for this message.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid KeepAliveRequestGuid { get; set; }

        /// <summary>
        /// Gets or sets the identifier of this server.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid ServerGuid { get; set; }

        /// <summary>
        /// Gets or sets the current CPU load in percent.
        /// </summary>
        /// <value>An integer.</value>
        public int CpuLoad { get; set; }

        /// <summary>
        /// Gets or sets the current number of connected clients.
        /// </summary>
        /// <value>An integer.</value>
        public int NumberOfClients { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the server is shutting down.
        /// </summary>
        /// <value>True if shutting down.</value>
        public bool Terminate { get; set; }
    }
}
