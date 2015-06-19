
namespace Subtract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Component;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Subtract
        : IComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ComponentGuid
        {
            get { return default(Guid); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FriendlyName
        {
            get { return "Subtract"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> InputHints
        {
            get
            {
                Type[] types = new Type[] { typeof(int), typeof(int) };

                foreach (Type t in types)
                {
                    yield return t.ToString();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> OutputHints
        {
            get
            {
                Type[] types = new Type[] { typeof(int) };

                foreach (Type t in types)
                {
                    yield return t.ToString();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> InputDescriptions
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> OutputDescriptions
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public IEnumerable<object> Evaluate(IEnumerable<object> values)
        {
            if (values.Count() != 2)
            {
                throw new ArgumentException();
            }

            yield return (int)values.First() - (int)values.Last();
        }
    }
}
