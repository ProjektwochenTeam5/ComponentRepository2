﻿// ----------------------------------------------------------------------- 
// <copyright file="ConsoleFloatOutput.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Component classlibary.</summary> 
// <author>Matthias Böhm</author> 
// -----------------------------------------------------------------------
namespace ConsoleFloatOutput
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Component;

    /// <summary>
    /// This is the component class for the console output.
    /// </summary>
    public class ConsoleFloatOutput : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleFloatOutput"/> class.
        /// </summary>
        public ConsoleFloatOutput()
        {
            this.ComponentGuid = Guid.NewGuid();
            this.InputHints = new ReadOnlyCollection<string>(new[] { typeof(float).ToString() });
            this.OutputHints = new List<string>();
            this.InputDescriptions = new List<string>(new[] { "float" });
            this.OutputDescriptions = new List<string>();     
        }

        /// <summary>
        /// Gets the unique component id.
        /// Must be generated by the Store.
        /// DO NOT REUSE GUIDS.
        /// </summary>
        /// <value>A unique identifier.</value>
        public Guid ComponentGuid
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the display name for the component.
        /// </summary>
        /// <value>A name string.</value>
        public string FriendlyName
        {
            get { return "Console float output"; }
        }

        /// <summary>
        /// Gets the collection of types that describe the input arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> InputHints
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection of types that describe the output arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> OutputHints
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the collection of strings that describe the input arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> InputDescriptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of strings that describe the output arguments.
        /// </summary>
        /// <value>Collection of strings.</value>
        public IEnumerable<string> OutputDescriptions
        {
            get;
            set;
        }

        /// <summary>
        /// Executes the implementation of the component.
        /// </summary>
        /// <param name="values">Collection of input arguments.</param>
        /// <returns>Collection of output arguments.</returns>
        public IEnumerable<object> Evaluate(IEnumerable<object> values)
        {
            if (values.Count() != 1)
            {
                return new object[] { new ArgumentException() };
            }
            
            float ioutput = 0;
            
            foreach (var item in values)
            {
                ioutput += (float)item;
            }

            Console.WriteLine("Please press enter to continue");
            Console.WriteLine(ioutput);
            Console.ReadKey(true);

            return new List<object>();
        }
    }
}
