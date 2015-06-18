// ----------------------------------------------------------------------- 
// <copyright file="ClientUpdateResponse.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the ClientUpdateResponse class.</summary> 
// <author>Michael Sabransky</author> 
// -----------------------------------------------------------------------
namespace Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Confirms a received client update message.
    /// </summary>
    [Serializable]
    public class ClientUpdateResponse
    {
        /// <summary>
        /// Gets or sets the unique id of the confirmed message.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid ClientUpdateRequestGuid { get; set; }
    }
}
