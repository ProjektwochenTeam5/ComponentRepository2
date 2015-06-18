// ----------------------------------------------------------------------- 
// <copyright file="JobResultResponse.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the JobResultResponse class.</summary> 
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
    /// Sent when a job result message was received.
    /// </summary>
    [Serializable]
    public class JobResultResponse
    {
        /// <summary>
        /// Gets or sets the unique id of the received message.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid JobResultGuid { get; set; }
    }
}
