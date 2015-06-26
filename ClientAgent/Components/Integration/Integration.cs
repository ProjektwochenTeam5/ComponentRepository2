﻿// ----------------------------------------------------------------------- 
// <copyright file="Integration.cs" company="FHWN"> 
// Copyright (c) FHWN. All rights reserved. 
// </copyright> 
// <summary>Component classlibary.</summary> 
// <author>Matthias Böhm</author> 
// -----------------------------------------------------------------------
namespace Integration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Component;

    /// <summary>
    /// This is the component class for numerical integration.
    /// </summary>
    public class Integration : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Integration"/> class.
        /// </summary>
        public Integration()
        {
            this.ComponentGuid = Guid.NewGuid();
            this.InputHints = new ReadOnlyCollection<string>(new[] { typeof(double).ToString(), typeof(double).ToString() });
            this.OutputHints = new ReadOnlyCollection<string>(new[] { typeof(double).ToString() });
            this.InputDescriptions = new List<string>(new[] { "lower limit", "upper limit"});
            this.OutputDescriptions = new List<string>(new[] { "double" });            
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
            get { return "numerical integration"; }
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
                yield return new object[] { new ArgumentException() };
            }
            double boarder1 = 0;
            double boarder2 = 0;
            double result = 0;
            
            Func<double, double> f = (Func<double, double>)values.First();
            double x = (double)values.Last();

            if (boarder1 <= boarder2)
            {
                yield return new object[] { new ArgumentException() };
            }
            else
            {
                result = ((boarder2 - boarder1) / 6) * (f(boarder1) + 4*f((boarder1 + boarder2) / 2 ) + f(boarder2));
            }
   
            yield return result;
        }
    }
}