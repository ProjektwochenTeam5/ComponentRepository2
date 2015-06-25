using ClientServerCommunication;
using Core.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The event that is raised when the underlying client informs about new components.
        /// </summary>
        public event EventHandler<ComponentEventArgs> OnComponentsReceived;

        /// <summary>
        /// The client used to communicate with the underlying client.
        /// </summary>
        private TcpClient client;

        /// <summary>
        /// The formatter used to serialize and deserialize messages.
        /// </summary>
        private BinaryFormatter formatter = new BinaryFormatter();

        /// <summary>
        /// The thread which listens for incoming messages.
        /// </summary>
        private Thread readingThread = null;

        private CommunicationThreadArgs readingThreadArgs = null;

        /// <summary>
        /// Sends a job request to the underlying client.
        /// </summary>
        /// <param name="job">The job to execute.</param>
        public void SendJobRequest(Component job)
        {
            //TODO send job request to agent
            DoJobRequest req = new DoJobRequest() { Job = job };
            SendMessage(req);
        }

        /// <summary>
        /// Sends a complex component to the underlying client.
        /// </summary>
        /// <param name="friendlyName">The friendly name of the component to send.</param>
        /// <param name="component">The component to send.</param>
        public void SendComplexComponent(string friendlyName, Component component)
        {
            //TODO send complex component
            StoreComponent storeReq = new StoreComponent();            
            MemoryStream ms = new MemoryStream();

            this.formatter.Serialize(ms, component);

            storeReq.FriendlyName = friendlyName;
            storeReq.Component = ms.ToArray();
            storeReq.IsComplex = true;
        }

        /// <summary>
        /// Connects to the underlying client using the provided port argument.
        /// If the port argument is omitted or the port is invalid, shows an error message
        /// and exits the program.
        /// </summary>
        /// <param name="e">The arguments of the startup event.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            int clientPort;

            base.OnStartup(e);            

            if (e.Args.Length != 1)
            {
                MessageBox.Show(
                    "Mandatory port number was not provided!\n\n" +
                    "The port must be the only parameter.\n" +
                    "The GUI needs the port number of the client to communicate.",
                    "ERROR", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);

                Environment.Exit(1);
            }
            
            if (!int.TryParse(e.Args[0], out clientPort))
            {
                MessageBox.Show(
                    "Passed port number could not be parsed.",
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Environment.Exit(2);
            }

            try
            {
                this.client = new TcpClient();
                this.client.Connect(new IPEndPoint(IPAddress.Loopback, clientPort));

                this.readingThread = new Thread(new ParameterizedThreadStart(HandleIncomingMessages));
                this.readingThreadArgs = new CommunicationThreadArgs(client);
                this.readingThread.Start(this.readingThreadArgs);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Environment.Exit(3);
            }
        }

        /// <summary>
        /// Sends a terminate signal to the underlying client and
        /// closes the connection to it before exiting.
        /// </summary>
        /// <param name="e">The arguments for the exit event.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            if (this.client != null)
            {
                try
                {
                    readingThreadArgs.Exit = true;
                    SendTerminateSignal();
                    this.client.GetStream().Close();
                    this.client.Close();
                }
                catch
                {
                }
            }

            base.OnExit(e);
            Environment.Exit(0);
        }

        /// <summary>
        /// Sends a keep alive message with terminate flag set to true
        /// to the underlying client.
        /// </summary>
        private void SendTerminateSignal()
        {
            KeepAlive terminateMsg = new KeepAlive() { Terminate = true };
            SendMessage(terminateMsg);
        }

        /// <summary>
        /// Sends a message to the client which started this GUI application.
        /// </summary>
        /// <param name="msg">The message to send.</param>
        private void SendMessage(Message msg)
        {
            if (this.client == null)
            {
                return;
            }

            MemoryStream ms = new MemoryStream();
            this.formatter.Serialize(ms, msg);

            uint length = (uint)ms.Length;
            byte b1, b100, b10000, b1000000;

            b1 = (byte)(length % 0x100);
            b100 = (byte)((length / 0x100) % 0x100);
            b10000 = (byte)((length / 0x10000) % 0x100);
            b1000000 = (byte)((length / 0x1000000) % 0x100);

            List<byte> byteMsg = new List<byte>();
            byteMsg.AddRange(new byte[] { 0, 0, 0, 0, b1, b100, b10000, b1000000, (byte)msg.MessageType });
            byteMsg.AddRange(ms.ToArray());

            if (this.client.Connected && this.client.GetStream().CanWrite)
            {
                this.client.GetStream().Write(byteMsg.ToArray(), 0, byteMsg.Count);
            }
        }

        /// <summary>
        /// Parses the incoming messages and raises the events.
        /// </summary>
        /// <param name="data"></param>
        private void HandleIncomingMessages(object data)
        {
            CommunicationThreadArgs args = data as CommunicationThreadArgs;
            NetworkStream stream = null;

            if (args == null)
            {
                throw new ArgumentException("Parameter must be of type CommuncationThreadArgs and must not be null.");
            }

            stream = args.Client.GetStream();

            while (!args.Exit)
            {
                if (stream.DataAvailable)
                {
                    byte[] hdr = new byte[9];

                    int hlen = stream.Read(hdr, 0, 9);
                    uint bodylen;
                    StatusCode messagType;

                    // go to next iteration if header lengh != 9
                    if (!ParseHeader(hdr, out bodylen, out messagType))
                    {
                        continue;
                    }

                    byte[] body = new byte[bodylen];
                    int rcvbody = stream.Read(body, 0, (int)bodylen);

                    using (MemoryStream ms = new MemoryStream(body))
                    {
                        Message rcv = (Message)this.formatter.Deserialize(ms);

                        switch (rcv.MessageType)
                        {
                            case StatusCode.SendComponentInfos:
                                SendComponentInfos compInfos = rcv as SendComponentInfos;

                                if (this.OnComponentsReceived != null)
                                {
                                    this.OnComponentsReceived(this, new ComponentEventArgs(compInfos.MetadataComponents));
                                }
                                break;
                        }
                    }
                }

                Thread.Sleep(20);
            }
        }

        /// <summary>
        /// Parses a byte array and returns whether the byte array is a valid header.
        /// </summary>
        /// <param name="header">
        ///     The byte array that shall be parsed.
        /// </param>
        /// <param name="length">
        ///     The length of the following body.
        /// </param>
        /// <param name="status">
        ///     The message status.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the specified byte array is a valid header.
        /// </returns>
        public static bool ParseHeader(byte[] header, out uint length, out StatusCode status)
        {
            length = 0;
            status = StatusCode.KeepAlive;

            if (header.Length != 9)
            {
                return false;
            }

            if (header[0] != 0 || header[1] != 0 || header[2] != 0 || header[3] != 0)
            {
                return false;
            }

            length = (uint)(header[4] + (header[5] * 0x100) + (header[6] * 0x10000) + (header[7] * 0x1000000));

            status = (StatusCode)header[8];
            return true;
        }
    }
}
