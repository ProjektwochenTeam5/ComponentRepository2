using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class SendComponentInfos : Message
    {
        public Dictionary<Guid,ComponentInfo> MetadataComponents { get; set; }
    }
}
