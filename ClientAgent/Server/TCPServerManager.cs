namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TCPServerManager
    {
        public TCPServer MyTCPServer { get; set; }

        public TCPServerManager()
        {
            this.MyTCPServer = new TCPServer();
            this.MyTCPServer.OnMessageRecieved += MyTCPServer_OnMessageRecieved;
        }

        void MyTCPServer_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch (e.MessageType)
            {
                case ClientServerCommunication.StatusCode.KeepAlive:
                    break;
                case ClientServerCommunication.StatusCode.AgentConnection:
                    break;
                case ClientServerCommunication.StatusCode.Acknowledge:
                    break;
                case ClientServerCommunication.StatusCode.TransferComponent:
                    break;
                case ClientServerCommunication.StatusCode.TransferJob:
                    break;
                case ClientServerCommunication.StatusCode.SendComponentInfos:
                    break;
                case ClientServerCommunication.StatusCode.DoJobRequest:
                    break;
                case ClientServerCommunication.StatusCode.StorComponent:
                    break;
                case ClientServerCommunication.StatusCode.Error:
                    break;
                default:
                    break;
            }
        }
    }
}
