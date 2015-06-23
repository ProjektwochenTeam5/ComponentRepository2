using Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerMessageReceivedEventArgs : EventArgs
    {
        public ServerMessageReceivedEventArgs(byte[] body, MessageCode code, TcpClient server)
        {
            this.MessageCode = code;
            this.MessageBody = body;
            this.Server = server;
        }

        public byte[] MessageBody { get; set; }

        public MessageCode MessageCode { get; set; }

        public TcpClient Server { get; set; }
    }
}
