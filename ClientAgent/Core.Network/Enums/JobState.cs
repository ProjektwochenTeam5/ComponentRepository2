// ----------------------------------------------------------------------- 
// <copyright file="JobState.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the JobState enum.</summary> 
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
    /// Contains possible states for the job result request message.
    /// </summary>
    public enum JobState
    {
        /// <summary>
        /// Job was completed successfully.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// An exception was thrown.
        /// </summary>
        Exception = 1,

        /// <summary>
        /// No clients available on the requested server.
        /// </summary>
        NoClients = 2,

        /// <summary>
        /// A component within a job started execution.
        /// </summary>
        ComponentStarted = 3,

        /// <summary>
        /// A component within a job finished execution.
        /// </summary>
        ComponentCompleted = 4,

        /// <summary>
        /// A component failed to execute.
        /// </summary>
        ComponentException = 5
    }
}
