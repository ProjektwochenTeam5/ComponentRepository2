using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerCommunication
{
    [Serializable]
    public class KeepAlive : Message
    {
        public double CPUWorkload { get; set; }
    }
}
