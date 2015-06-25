using ClientServerCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ServerReceiver
    {
        public TcpListener Listener { get; set; }

        public Guid ServerGuid { get; set; }

        public Dictionary<Guid, IPEndPoint> Servers { get; set; }

        public string FriendlyName { get; set; }

        public event EventHandler<ServerMessageReceivedEventArgs> OnMessageRecieved;

        public ServerReceiver()
        {
            this.ServerGuid = Guid.NewGuid();
            this.Listener = new TcpListener(IPAddress.Any, 3654);
            this.Servers = new Dictionary<Guid, IPEndPoint>();
            this.FriendlyName = "Team 5 Server";
        }

        public void StartReceiving()
        {
            try
            {
                Console.WriteLine("Suche nach anderen Servern...");
                this.Listener.Start();

                while (true)
                {
                    TcpClient client = this.Listener.AcceptTcpClient();
                    Console.WriteLine("Server da.");
                    Thread clientThread = new Thread(new ParameterizedThreadStart(ClientWorker));
                    clientThread.IsBackground = true;
                    clientThread.Start(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ClientWorker(object obj)
        {
            TcpClient client = (TcpClient)obj;

            NetworkStream ns = client.GetStream();

            while (true)
            {
                bool cont = true;
                byte[] body = null;
                byte messagetype = 0;

                if (ns.DataAvailable)
                {
                    int index = 0;
                    while (cont)
                    {
                        byte[] buffer = new byte[1024];

                        int recievedbytes = ns.Read(buffer, 0, buffer.Length);
                        Console.WriteLine(recievedbytes + " bytes empfangen");

                        if (body == null)
                        {
                            byte[] messageLength = new byte[4];

                            for (int i = 0; i < 5; i++)
                            {
                                messageLength[i] = buffer[1 + i];
                            }

                            var length = BitConverter.ToInt32(messageLength, 0) + 5;
                            messagetype = buffer[0];
                            body = new byte[length - 5];

                            for (int i = 5; i < recievedbytes; i++)
                            {
                                body[index] = buffer[i];
                                index++;
                            }

                            if (recievedbytes == buffer.Length)
                            {
                                continue;
                            }
                            else
                            {
                                this.FireOnMessageRecieved(new ServerMessageReceivedEventArgs(body, (Core.Network.MessageCode)messagetype, client));
                                break;
                            }
                        }

                        for (int i = 0; i < recievedbytes; i++)
                        {
                            body[index] = buffer[i];
                            index++;
                        }

                        if (index >= body.Length)
                        {
                            this.FireOnMessageRecieved(new ServerMessageReceivedEventArgs(body, (Core.Network.MessageCode)messagetype, client));
                            break;
                        }
                    }
                }
            }
        }

        protected void FireOnMessageRecieved(ServerMessageReceivedEventArgs e)
        {
            if (this.OnMessageRecieved != null)
            {
                this.OnMessageRecieved(this, e);
            }
        }
    }
}
