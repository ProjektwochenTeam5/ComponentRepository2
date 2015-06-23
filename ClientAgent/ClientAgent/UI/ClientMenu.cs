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

    /// <summary>
    /// Provides the client's main menu.
    /// </summary>
    public class ClientMenu
        : Menu
    {
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
            throw new NotImplementedException();
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
