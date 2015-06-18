using Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ExtendedComponentEdge : ComponentEdge
    {
        public ExtendedComponentEdge(ComponentEdge edge)
        {
            this.InputComponentGuid = edge.InputComponentGuid;
            this.InputValueID = edge.InputValueID;
            this.InternalInputComponentGuid = edge.InternalInputComponentGuid;
            this.InternalOutputComponentGuid = edge.InternalOutputComponentGuid;
            this.OutputComponentGuid = edge.OutputComponentGuid;
            this.OutputValueID = edge.OutputValueID;
            this.ComponentResult = null;
        }

        public object ComponentResult { get; set; }
    }
}
