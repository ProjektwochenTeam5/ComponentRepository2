using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageRecievedEventArgs(byte[] message, int messagetype)
        {
            this.MessageBody = message;
            this.MessageType = messagetype;
        }

        public byte[] MessageBody { get; set; }

        public int MessageType { get; set; }
    }
}
