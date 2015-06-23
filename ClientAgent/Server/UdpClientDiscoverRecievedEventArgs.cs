using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class UdpClientDiscoverRecievedEventArgs : EventArgs
    {
        public string FriendlyName { get; set; }

        public string IPAdress { get; set; }

        public UdpClientDiscoverRecievedEventArgs(string ipadress, string friendlyname)
        {
            this.IPAdress = ipadress;
            this.FriendlyName = friendlyname;
        }
    }
}
