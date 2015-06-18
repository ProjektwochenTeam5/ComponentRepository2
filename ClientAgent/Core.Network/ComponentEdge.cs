// ----------------------------------------------------------------------- 
// <copyright file="ComponentEdge.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the ComponentEdge class.</summary> 
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
    /// Represents an edge between 2 components.
    /// </summary>
    /// <example>
    /// Output Comp         InputComp
    ///  --------        ----------
    ///  |I1     |       | I1     |
    ///  |    O1 | --->  | I2   O1|   
    ///  |I2     |       | I3     |
    ///  ---------       ----------
    ///  OutputValueId = 1.
    ///  InputValueId = 2.
    /// </example>
    [Serializable]
    public class ComponentEdge
    {
        /// <summary>
        ///  Gets or sets the unique identifier of the source component.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid OutputComponentGuid { get; set; }

        /// <summary>
        /// Gets or sets the unique internal identifier of the source component.
        /// </summary>
        /// <value>A unique identifier.</value>
        /// <remarks>
        /// Identifies the component within a component graph.
        /// Use Guid.Empty to identifiy an output value within the output values enumerable for a component.
        /// </remarks>
        public Guid InternalOutputComponentGuid { get; set; }

        /// <summary>
        /// Gets or sets the unique internal identifier of the target component.
        /// </summary>
        /// <value>A unique identifier.</value>
        /// <remarks>
        /// Identifies the component within a component graph.
        /// Use Guid.Empty to identifiy the an input value within the input values enumerable for a component.
        /// </remarks>
        public Guid InternalInputComponentGuid { get; set; }

        /// <summary>
        ///  Gets or sets the unique identifier of the destination component.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid InputComponentGuid { get; set; }

        /// <summary>
        /// Gets or sets the source component output port number.
        /// </summary>
        /// <value>An unsigned integer.</value>
        public uint OutputValueID { get; set; }

        /// <summary>
        /// Gets or sets the destination component input port number.
        /// </summary>
        /// <value>An unsigned integer.</value>
        public uint InputValueID { get; set; }
    }
}
