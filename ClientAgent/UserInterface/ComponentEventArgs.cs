using Core.Network;
using System;
using System.Collections.Generic;

namespace UserInterface
{
    public class ComponentEventArgs : EventArgs
    {
        public ComponentEventArgs(ICollection<Component> components)
            : base()
        {
            this.Components = components;
        }

        public ICollection<Component> Components
        {
            get;
            private set;
        }
    }
}
