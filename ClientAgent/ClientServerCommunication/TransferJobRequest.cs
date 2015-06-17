using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class TransferJobRequest : Message
    {
        public byte[] Job { get; set; }

        public Guid ServerID { get; set; }

        public Guid ClientID { get; set; }
    }
}
