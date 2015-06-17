// ----------------------------------------------------------------------- 
// <copyright file="ComponentSubmitResponse.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the ComponentSubmitResponse class.</summary> 
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
    /// Confirms a received component submit message.
    /// </summary>
    public class ComponentSubmitResponse
    {
        /// <summary>
        /// Gets or sets the unique id of the confirmed message.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid ComponentSubmitRequestGuid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the component was accepted.
        /// </summary>
        /// <value>True if accepted.</value>
        public bool IsAccepted { get; set; }
    }
}
