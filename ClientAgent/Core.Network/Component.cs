// ----------------------------------------------------------------------- 
// <copyright file="Component.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the Component class.</summary> 
// <author>Michael Sabransky</author> 
// -----------------------------------------------------------------------
namespace Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a component as viewed by a server.
    /// Can describe an atomic or complex component.
    /// </summary>
    [Serializable]
    public class Component 
    {

        /// <summary>
        /// Gets or sets the unique identifier of the component.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid ComponentGuid { get; set; }

        /// <summary>
        /// Gets or sets the display name of the component.
        /// </summary>
        /// <value>A name string.</value>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the collection of strings that describe the input argument types.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> InputHints { get; set; }

        /// <summary>
        /// Gets or sets the collection of types that represent the output argument types.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> OutputHints { get; set; }

        /// <summary>
        /// Gets or sets the collection of strings that describe the input arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> InputDescriptions { get; set; }

        /// <summary>
        /// Gets or sets the collection of strings that describe the output arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> OutputDescriptions { get; set; }

        /// <summary>
        /// Gets or sets a collection of edges which describe the graph.
        /// Will be empty if the component is atomic.
        /// </summary>
        /// <value>A collection of ComponentEdge.</value>
        public IEnumerable<ComponentEdge> Edges { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this component is atomic.
        /// </summary>
        /// <value>True if atomic.</value>
        public bool IsAtomic { get; set; }
    }
}
