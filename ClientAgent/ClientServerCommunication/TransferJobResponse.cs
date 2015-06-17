using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class TransferJobResponse : Message
    {
        public ComponentInfo Type { get; set; }

        public object[] Result { get; set; }
    }
}
