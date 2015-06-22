using System;
using Core.Network;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class SendJobToClientEventArgs : EventArgs
    {
        public SendJobToClientEventArgs(Guid componentGuid, List<object> inputdata)
        {
            this.ComponentGuid = componentGuid;
            this.InputData = inputdata;
        }

        public SendJobToClientEventArgs()
        {
        }

        public Guid ComponentGuid { get; set; }

        public List<object> InputData { get; set; }
    }
}
