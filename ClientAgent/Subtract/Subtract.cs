using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Component;

namespace Subtract
{
    public class Subtract
        : IComponent
    {
        public Guid ComponentGuid
        {
            get { return new Guid(); }
        }

        public string FriendlyName
        {
            get { return "Subtract"; }
        }

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

        public IEnumerable<string> InputDescriptions
        {
            get;
            set;
        }

        public IEnumerable<string> OutputDescriptions
        {
            get;
            set;
        }

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
