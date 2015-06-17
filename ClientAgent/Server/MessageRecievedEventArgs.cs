using Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageRecievedEventArgs(byte[] message, int messagetype, ClientInfo info)
        {
            this.MessageBody = message;
            this.MessageType = messagetype;
            this.Info = info;
        }

        public byte[] MessageBody { get; set; }

        public int MessageType { get; set; }

        public ClientInfo Info { get; set; }
    }
}
