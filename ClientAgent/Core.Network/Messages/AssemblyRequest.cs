// ----------------------------------------------------------------------- 
// <copyright file="AssemblyRequest.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the AssemblyRequest class.</summary> 
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
    /// Requests an Assembly.
    /// The Response is BINARY ENCODED!
    /// When there is a problem with the assembly (e.g. cannot be found) the length is set to 0.
    /// </summary>
    [Serializable]
    public class AssemblyRequest
    {
        /// <summary>
        /// Gets or sets the unique id for this message.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid AssemblyRequestGuid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the requested component.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid ComponentGuid { get; set; }
    }
}
