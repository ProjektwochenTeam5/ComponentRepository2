// ----------------------------------------------------------------------- 
// <copyright file="ClientInfo.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the ClientInfo class.</summary> 
// <author>Michael Sabransky</author> 
// -----------------------------------------------------------------------
namespace Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Information about a client as viewed by a server.
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// Gets or sets the unique identifier of the client.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid ClientGuid { get; set; }

        /// <summary>
        /// Gets or sets the display name of the client.
        /// </summary>
        /// <value>A name string.</value>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the client.
        /// </summary>
        /// <value>An IP address.</value>
        public IPAddress IpAddress { get; set; }
    }
}
