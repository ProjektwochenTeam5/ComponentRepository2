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
        public Add()
        {
            this.ComponentGuid = new Guid();
            this.InputHints = new List<string>();
            this.OutputHints = new List<string>();

            this.InputHints.ToList().Add(typeof(int).ToString());
            this.InputHints.ToList().Add(typeof(int).ToString());

            this.OutputHints.ToList().Add(typeof(int).ToString());
        }

        public Guid ComponentGuid
        {
            get;
            private set;
        }

        public string FriendlyName
        {
            get;
            private set;
        }

        public IEnumerable<string> InputHints
        {
            get;
            private set;
        }

        public IEnumerable<string> OutputHints
        {
            get;
            private set;
        }

        public IEnumerable<object> Evaluate(IEnumerable<object> values)
        {
            int result = 0;

            foreach (var item in values)
            {
                result += (int)item;
            }

            yield return result;
        }
    }
}
