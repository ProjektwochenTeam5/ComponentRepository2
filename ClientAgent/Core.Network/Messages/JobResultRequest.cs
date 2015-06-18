// ----------------------------------------------------------------------- 
// <copyright file="JobResultRequest.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the JobResultRequest class.</summary> 
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
    /// Sent when a job was completed, failed or could not be executed.
    /// </summary>
    [Serializable]
    public class JobResultRequest
    {
        /// <summary>
        /// Gets or sets the unique id for this message.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid JobResultGuid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the job.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid JobGuid { get; set; }

        /// <summary>
        /// Gets or sets the state of the job.
        /// </summary>
        /// <value>A JobState.</value>
        public JobState State { get; set; }

        /// <summary>
        /// Gets or sets a collection of output arguments.
        /// </summary>
        /// <value>If State ==  Exception (1) || NoClient (2) --> 1st Element == string == error message.
        /// If State == ComponentStarted (3) || ComponentCompleted (4) || ComponentException (5) --> 1st Element == error message, second element == InternalComponent unique identifier.
        /// Otherwise Result.
        /// </value>
        public IEnumerable<object> OutputData { get; set; }
    }
}
