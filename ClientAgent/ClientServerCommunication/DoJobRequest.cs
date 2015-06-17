using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class DoJobRequest : Message
    {
        public byte[] Job { get; set; }

        public Guid? TargetDisplay { get; set; }

    }
}
