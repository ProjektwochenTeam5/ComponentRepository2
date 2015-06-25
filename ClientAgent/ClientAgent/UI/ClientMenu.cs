// --------------------------------------------------------------
// <copyright file="ClientMenu.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="ClientMenu"/> class.
// </summary>
// <author>
// David Eiwen
// </author>
// --------------------------------------------------------------

namespace ClientAgent.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using ConsoleGUI.Controls;
    using ConsoleGUI.IO;
    using System.Threading;
    using System.Diagnostics;
    using ClientServerCommunication;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Provides the client's main menu.
    /// </summary>
    public class ClientMenu
        : Menu
    {
        private static int lastPort = 12000;

        /// <summary>
        /// The 'Create Component' button.
        /// </summary>
        private MenuButton buttonCreateComponent;

        /// <summary>
        /// The 'Show Components' button.
        /// </summary>
        private MenuButton buttonShowComponents;

        /// <summary>
        /// The 'Show Jobs' button.
        /// </summary>
        private MenuButton buttonShowJobs;

        /// <summary>
        /// The 'Quit' button.
        /// </summary>
        private MenuButton buttonQuit;

        /// <summary>
        /// The status label.
        /// </summary>
        private Label labelStatus;

        /// <summary>
        /// The statuc stack text box.
        /// </summary>
        private StackTextBox stacktextboxStatus;

        /// <summary>
        /// The thread responsible for communicating wth the GUI.
        /// </summary>
        private Thread guiThread;

        /// <summary>
        /// The thread arguments for the GUI thread.
        /// </summary>
        private ThreadArgs guiThreadArgs;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMenu"/> class.
        /// </summary>
        /// <param name="outputs">
        ///     The outputs for the menu.
        /// </param>
        /// <param name="src">
        ///     The input source for the menu.
        /// </param>
        /// <param name="c">
        ///     The client the menu is working with.
        /// </param>
        /// <param name="parent">
        ///     The parent menu.
        /// </param>
        public ClientMenu(ICollection<IRenderer> outputs, IInputSource src, TcpClient c, Menu parent = null)
            : base(outputs, src, parent)
        {
            // Set Properties
            // ++++++++++
            // Open Gui button
            this.buttonCreateComponent = new MenuButton() { Enabled = true, LinkedKey = ConsoleKey.F2, Text = "F2 Open GUI", Visible = true };
            this.buttonCreateComponent.ButtonKeyPressed += this.ButtonCreateComponent_ButtonKeyPressed;

            // Show Comonents button
            this.buttonShowComponents = new MenuButton() { Enabled = true, LinkedKey = ConsoleKey.F3, Text = "F3 Components", Visible = true };
            this.buttonShowComponents.ButtonKeyPressed += this.ButtonShowComponents_ButtonKeyPressed;

            // Show Curent Jobs button
            this.buttonShowJobs = new MenuButton() { Enabled = true, LinkedKey = ConsoleKey.F4, Text = "F4 Jobs", Visible = true };
            this.buttonShowJobs.ButtonKeyPressed += this.ButtonShowJobs_ButtonKeyPressed;
            
            // Quit Button
            this.buttonQuit = new MenuButton() { Enabled = true, LinkedKey = ConsoleKey.F12, Text = "F12 Quit", Visible = true };
            this.buttonQuit.ButtonKeyPressed += this.ButtonQuit_ButtonKeyPressed;

            // Status Label
            this.labelStatus = new Label(this.Renderers);
            this.labelStatus.Text = string.Empty;
            this.labelStatus.Rectangle = new ConsoleGUI.Rectangle(0, 0, 64, 5);

            // Satus Stack Text Box
            /*this.stacktextboxStatus = new StackTextBox(outputs);
            this.stacktextboxStatus.BackgroundColor = ConsoleColor.Blue;
            this.stacktextboxStatus.ForegroundColor = ConsoleColor.White;*/

            // Add buttons
            this.Buttons.AddRange(new[] { this.buttonCreateComponent, this.buttonShowComponents, this.buttonShowJobs, this.buttonQuit });

            // Add controls
            this.Controls.Add(this.labelStatus);

            // Add Input Receivers
            // ++++++++++
            // Events
            // ++++++++++
            // Initialize Client
            this.Client = new Client(c);
            this.Client.Connected += this.Client_Connected;
            this.Manager = new ClientMessageManager(this.Client);
        }

        /// <summary>
        /// Gets the client of the menu.
        /// </summary>
        /// <value>
        ///     Contains the client of the menu.
        /// </value>
        public Client Client
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the manager for the client.
        /// </summary>
        /// <value>
        ///     Contains the manager for the client.
        /// </value>
        public ClientMessageManager Manager
        {
            get;
            private set;
        }

        /// <summary>
        /// Starts the underlying client.
        /// </summary>
        public void StartWork()
        {
            this.Client.StartConnectionSearch();
            this.labelStatus.Text = "Started Connection search...";
        }

        /// <summary>
        /// Stops the underlying client.
        /// </summary>
        public void StopWork()
        {
            this.Client.StopAll();
        }

        /// <summary>
        /// Called when the linked key of the 'Show Jobs' button was pressed.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void ButtonShowJobs_ButtonKeyPressed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the linked key of the 'Show Components' button was pressed.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void ButtonShowComponents_ButtonKeyPressed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the linked key of the 'Create Component' button was pressed.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void ButtonCreateComponent_ButtonKeyPressed(object sender, EventArgs e)
        {
            if (this.guiThread != null && !this.guiThread.IsAlive)
            {
                // stop
                this.guiThreadArgs.Stop();
                this.buttonCreateComponent.Text = "F2 Open GUI";
                return;
            }

            this.guiThread = new Thread(this.GuiCommunication);
            this.guiThreadArgs = new ThreadArgs();
            this.guiThread.Start(this.guiThreadArgs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void GuiCommunication(object data)
        {
            ThreadArgs args = (ThreadArgs)data;

            Process c = new Process();
            c.StartInfo.FileName = "UserInterface.exe";

            int port = 0;
            TcpListener l;
            do
            {
                try
                {
                    port = lastPort;
                    l = new TcpListener(new IPEndPoint(IPAddress.Any, lastPort++));
                    l.Start();
                    if (lastPort >= 15000)
                    {
                        lastPort = 12000;
                    }

                    break;
                }
                catch
                {
                    Thread.Sleep(20);
                }
            }
            while (true);

            c.StartInfo.Arguments = port.ToString();
            c.StartInfo.CreateNoWindow = false;
            c.Start();

            if (!c.HasExited)
            {
                TcpClient cl = l.AcceptTcpClient();
                BinaryFormatter f = new BinaryFormatter();

                SendComponentInfos sci = new SendComponentInfos();
                sci.MetadataComponents = this.Manager.StoredComponentInfos.ToArray();
                MemoryStream ms = new MemoryStream();

                f.Serialize(ms, sci);
                long length = ms.Length;

                byte b1, b100, b10000, b1000000;

                b1 = (byte)(length % 0x100);
                b100 = (byte)((length / 0x100) % 0x100);
                b10000 = (byte)((length / 0x10000) % 0x100);
                b1000000 = (byte)((length / 0x1000000) % 0x100);

                List<byte> send = new List<byte>(new byte[] { 0, 0, 0, 0, b1, b100, b10000, b1000000, (byte)sci.MessageType });
                send.AddRange(ms.ToArray());

                NetworkStream nstr = cl.GetStream();
                nstr.Write(send.ToArray(), 0, send.Count);

                // wait for answer
                while (nstr.CanRead && nstr.CanWrite)
                {
                    if (nstr.DataAvailable)
                    {
                        byte[] hdr = new byte[9];

                        nstr.Read(hdr, 0, 9);

                        uint len;
                        StatusCode sc;

                        if (!Client.ParseHeader(hdr, out len, out sc))
                        {
                            continue;
                        }

                        byte[] body = new byte[len];
                        int rcvbody = nstr.Read(body, 0, (int)len);

                        using (MemoryStream nms = new MemoryStream(body))
                        {
                            Message rcv = (Message)f.Deserialize(nms);
                            switch (rcv.MessageType)
                            {
                                case StatusCode.ExecuteJob:
                                    ExecuteRequest eq = rcv as ExecuteRequest;
                                    this.Client.SendMessage(eq);
                                    break;

                                case StatusCode.StorComponent:
                                    StoreComponent stc = rcv as StoreComponent;
                                    stc.IsComplex = true;
                                    this.Client.SendMessage(stc);
                                    break;

                                case StatusCode.KeepAlive:
                                    KeepAlive ka = rcv as KeepAlive;
                                    if (ka.Terminate)
                                    {
                                        nstr.Close();
                                        cl.Close();
                                    }

                                    break;
                            }
                        }
                    }

                    Thread.Sleep(10);
                }

                try
                {
                    cl.Client.Disconnect(false);
                }
                catch
                {
                }
                finally
                {
                    l.Stop();
                }

                c.WaitForExit();
            }

            l.Stop();
        }

        /// <summary>
        /// Called when the linked key of the 'Quit' button was pressed.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void ButtonQuit_ButtonKeyPressed(object sender, EventArgs e)
        {
            this.StopWork();
            this.Close();
        }

        /// <summary>
        /// Called when the client has connected to a server.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for this event.
        /// </param>
        private void Client_Connected(object sender, EventArgs e)
        {
            this.labelStatus.Text = "Connected to a server!";
        }
    }
}
