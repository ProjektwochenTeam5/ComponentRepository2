using Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class JobEventArgs : EventArgs
    {
        public JobEventArgs(Component component, Guid? targetDisplay, Guid? targetCalc, List<object> inputData, string friendly, Guid targetServer)
        {
            this.MyComponent = component;
            this.TargetCalc = targetCalc;
            this.TargetDisplay = targetDisplay;
            this.InputData = inputData;
            this.Friendly = friendly;
            this.TargetServer = targetServer;
        }

        public Component MyComponent { get; set; }

        public Guid? TargetDisplay { get; set; }

        public Guid? TargetCalc { get; set; }

        public List<object> InputData { get; set; }
        public string Friendly { get; set; }

        public Guid TargetServer { get; set; }
    }
}
