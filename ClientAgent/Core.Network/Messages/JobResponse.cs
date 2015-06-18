// ----------------------------------------------------------------------- 
// <copyright file="JobResponse.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the JobResponse class.</summary> 
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
    /// Sent when a job request was received.
    /// </summary>
    [Serializable]
    public class JobResponse
    {
        /// <summary>
        /// Gets or sets the unique id of the received message.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid JobRequestGuid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the job was accepted.
        /// Usually true - only when there are no clients, hop count too high etc. false.
        /// </summary>
        /// <value>True if accepted.</value>
        public bool IsAccepted { get; set; }
    }
}
