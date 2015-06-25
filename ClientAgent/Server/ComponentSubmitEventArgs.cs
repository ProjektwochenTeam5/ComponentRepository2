using Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class ComponentSubmitEventArgs : EventArgs
    {
        public ComponentSubmitEventArgs(Component component)
        {
            this.MyComponent = component;
        }

        public Component MyComponent { get; set; }
    }
}
