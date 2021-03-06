﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class ComponentInfo
    {        /// <summary>
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
        /// Gets or sets the collection of types that describe the input arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> InputHints { get; set; }

        /// <summary>
        /// Gets or sets the collection of types that describe the output arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> OutputHints { get; set; }
    }
}
