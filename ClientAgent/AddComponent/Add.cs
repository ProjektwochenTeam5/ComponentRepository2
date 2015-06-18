using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Component;

namespace AddComponent
{
    public class Add : IComponent
    {
        public Guid ComponentGuid
        {
            get { return new Guid(); }
        }

        public string FriendlyName
        {
            get { return "Addieren"; }
        }

        public IEnumerable<string> InputHints
        {
            get
            {
                List<string> inputs = new List<string>();
                inputs.Add(typeof(int).ToString());
                inputs.Add(typeof(int).ToString());

                foreach (var item in inputs)
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<string> OutputHints
        {
            get { yield return typeof(int).ToString(); }
        }

        public IEnumerable<object> Evaluate(IEnumerable<object> values)
        {
            int first = 0;
            bool anfang = true;
            foreach (var item in values)
            {
                if (anfang)
                {
                    first = (int)item;
                    anfang = false;
                }
                else
                {
                    yield return first + (int)item;
                }
            }
        }
    }
}
