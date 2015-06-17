using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class StoreComponent : Message
    {
        public byte[] Component { get; set; }
    }
}
