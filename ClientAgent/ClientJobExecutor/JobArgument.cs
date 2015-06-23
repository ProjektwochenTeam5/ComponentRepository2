using Core.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientJobExecutor
{
    [Serializable]
    public class JobArgument
    {
        public JobArgument(IEnumerable<object> inputArgs, IComponent component)
        {
            this.Component = component;
            this.InputArguments = inputArgs;
        }

        public IComponent Component
        {
            get;
            private set;
        }

        public IEnumerable<object> InputArguments
        {
            get;
            private set;
        }
    }
}
