// ----------------------------------------------------------------------- 
// <copyright file="ClientState.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Client states for server communication.</summary> 
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
    /// Contains possible states for the client update message.
    /// </summary>
    public enum ClientState
    {
        /// <summary>
        /// A client was connected.
        /// </summary>
        Connected,

        /// <summary>
        /// A client was disconnected.
        /// </summary>
        Disconnected
    }
}
