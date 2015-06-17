// ----------------------------------------------------------------------- 
// <copyright file="LogonResponse.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the LogonResponseclass.</summary> 
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
    /// Sent when a logon request message was received.
    /// </summary>
    public class LogonResponse 
    {
        /// <summary>
        /// Gets or sets the unique id of the received message.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid LogonRequestGuid { get; set; }

        /// <summary>
        /// Gets or sets my unique identifier.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid ServerGuid { get; set; }

        /// <summary>
        /// Gets or sets my display name.
        /// </summary>
        /// <value>A name string.</value>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the current collection of available components in my store.
        /// </summary>
        /// <value>A collection of Component.</value>
        public IEnumerable<Component> AvailableComponents { get; set; }

        /// <summary>
        /// Gets or sets a collection of currently connected clients.
        /// </summary>
        /// <value>A collection of ClientInfo.</value>
        public IEnumerable<ClientInfo> AvailableClients { get; set; }
    }
}
