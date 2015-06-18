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

        public List<TcpClient> Servers { get; set; }

        public event EventHandler<MessageRecievedEventArgs> OnMessageRecieved;

        public ServerReceiver()
        {
            this.ServerGuid = new Guid();
            this.Listener = new TcpListener(IPAddress.Any, 8080);
            this.Servers = new List<TcpClient>();
        }

        public void StartReceiving()
        {
            try
            {
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
            this.Servers.Add(client);

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

                            for (int i = 0; i < 4; i++)
                            {
                                messageLength[i] = buffer[4 + i];
                            }

                            var length = BitConverter.ToInt32(messageLength, 0) + 9;
                            messagetype = buffer[8];
                            body = new byte[length - 9];

                            for (int i = 9; i < recievedbytes; i++)
                            {
                                body[index] = buffer[i];
                                index++;
                            }

                            if (recievedbytes == buffer.Length)
                            {
                                continue;
                            }
                            //else
                            //{
                            //    this.FireOnMessageRecieved(new MessageRecievedEventArgs(body, (StatusCode)messagetype, clientInfo));
                            //    break;
                            //}
                        }

                        for (int i = 0; i < recievedbytes; i++)
                        {
                            body[index] = buffer[i];
                            index++;
                        }

                        //if (index >= body.Length)
                        //{
                        //    this.FireOnMessageRecieved(new MessageRecievedEventArgs(body, (StatusCode)messagetype, clientInfo));
                        //    break;
                        //}
                    }
                }
            }
        }

        protected void FireOnMessageRecieved(MessageRecievedEventArgs e)
        {
            if (this.OnMessageRecieved != null)
            {
                this.OnMessageRecieved(this, e);
            }
        }
    }
}
