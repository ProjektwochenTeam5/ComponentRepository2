﻿// ----------------------------------------------------------------------- 
// <copyright file="Fibo.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Component classlibary.</summary> 
// <author>Matthias Böhm</author> 
// -----------------------------------------------------------------------
namespace Fibo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Component;

    /// <summary>
    /// This is the component class to get Fibonacci numbers.
    /// </summary>
    public class Fibo : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Fibo"/> class.
        /// </summary>
        public Fibo()
        {
            this.ComponentGuid = Guid.NewGuid();
            this.InputHints = new ReadOnlyCollection<string>(new[] { typeof(long).ToString() });
            this.OutputHints = new ReadOnlyCollection<string>(new[] { typeof(long).ToString() });
            this.InputDescriptions = new List<string>(new[] { "long" });
            this.OutputDescriptions = new List<string>(new[] { "long" });            
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
            get { return "Fibonacci Numbers"; }
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
            if (values.Count() != 2)
            {
                return new object[] { new ArgumentException() };
            }
           
            long a = 0;
            long b = 1;
            long n = 0;

            for (int i = 0; i < n; i++)
            {
                long temp = a;
                a = b;
                b = temp + b;
            }

            return new object[] { a };
        }
    }
}