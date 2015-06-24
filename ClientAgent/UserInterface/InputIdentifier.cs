using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    public class InputIdentifier
    {
        public InputIdentifier(Guid internalComponentGuid, uint inputPort)
        {
            this.InternalComponentGuid = internalComponentGuid;
            this.InputPort = inputPort;
        }

        public Guid InternalComponentGuid
        {
            get;
            set;
        }

        public uint InputPort
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            InputIdentifier other = obj as InputIdentifier;

            if (other != null)
            {
                return
                    other.InternalComponentGuid == this.InternalComponentGuid &&
                    other.InputPort == this.InputPort;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.InternalComponentGuid.GetHashCode() + (int)this.InputPort;
        }
    }
}
