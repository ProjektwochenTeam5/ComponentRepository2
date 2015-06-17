// ----------------------------------------------------------------------- 
// <copyright file="MessageCode.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Contains the MessageCode enum.</summary> 
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
    /// Contains possible values for the message code header field.
    /// Used for both requests and responses.
    /// </summary>
    public enum MessageCode
    {
        /// <summary>
        /// Logon message.
        /// </summary>
        Logon = 1,

        /// <summary>
        /// Keep alive message.
        /// </summary>
        KeepAlive = 2,

        /// <summary>
        /// Component submit message.
        /// </summary>
        ComponentSubmit = 3,

        /// <summary>
        /// Job request message.
        /// </summary>
        JobRequest = 4,

        /// <summary>
        /// Job result message.
        /// </summary>
        JobResult = 5,

        /// <summary>
        /// Assembly request message.
        /// </summary>
        RequestAssembly = 6,

        /// <summary>
        /// Client update message.
        /// </summary>
        ClientUpdate = 7
    }
}
