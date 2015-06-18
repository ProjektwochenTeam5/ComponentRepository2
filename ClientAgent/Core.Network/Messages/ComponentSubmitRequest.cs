// ----------------------------------------------------------------------- 
// <copyright file="ComponentSubmitRequest.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the ComponentSubmitRequest class.</summary> 
// <author>Michael Sabransky</author> 
// -----------------------------------------------------------------------
namespace Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Notify the other servers about a new component.
    /// </summary>
    [Serializable]
    public class ComponentSubmitRequest 
    {
        /// <summary>
        /// Gets or sets the unique id for this message.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid ComponentSubmitRequestGuid { get; set; }

        /// <summary>
        /// Gets or sets the new component meta data.
        /// </summary>
        /// <value>A Component.</value>
        public Core.Network.Component Component { get; set; }
    }
}
