namespace Server
{
    using ClientServerCommunication;
using Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class TCPServerManager
    {
        public TCPServer MyTCPServer { get; set; }

        public Dictionary<Guid, double> CPULoads { get; set; }

        public double AllClientLoad { get; set; }

        public TCPServerManager()
        {
            this.MyTCPServer = new TCPServer();
            this.MyTCPServer.OnMessageRecieved += this.MyTCPServer_OnMessageRecieved;
        }

        public void RunMyServer()
        {
            this.MyTCPServer.Run();
        }

        void MyTCPServer_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch (e.MessageType)
            {
                case ClientServerCommunication.StatusCode.KeepAlive:
                    {
                        KeepAlive keepAlive = DataConverter.ConvertByteArrrayToKeepAlive(e.MessageBody);
                        this.CalculateClientLoads(keepAlive);
                        this.CheckIfDeleteClientAndDelete(keepAlive, e.Info);
                        break;
                    }

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
                    {
                        StoreComponent storecomponent = DataConverter.ConvertByteArrayToStoreComponent(e.MessageBody);
                        DataBaseWrapper db = new DataBaseWrapper();
                        db.StoreComponent(storecomponent.Component);
                        break;
                    }
                case ClientServerCommunication.StatusCode.Error:
                    break;
                default:
                    break;
            }
        }

        private void CalculateClientLoads(KeepAlive keepAlive)
        {
            Console.WriteLine("Keep alive empfangen + terminnate = {0}", keepAlive.Terminate.ToString());
        }

        private void CheckIfDeleteClientAndDelete(KeepAlive ka, ClientInfo info)
        {
            if (ka.Terminate)
            {
                this.MyTCPServer.Clients.Remove(info);
                Console.WriteLine("Client deleted!");
            }
            else
            {
            }
        }
    }
}
