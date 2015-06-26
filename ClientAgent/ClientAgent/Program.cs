// --------------------------------------------------------------
// <copyright file="Program.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the static <see cref="Program"/> class providing the client's main entry point.
// </summary>
// <author>
// David Eiwen
// </author>
// --------------------------------------------------------------

namespace ClientAgent
{
    using System;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using ConsoleGUI.IO;
    using UI;

    /// <summary>
    /// Provides the main entry point of the client
    /// as well as global variables for the client.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The console input source.
        /// </summary>
        private static ConsoleInput input;

        /// <summary>
        /// The client's main menu.
        /// </summary>
        private static ClientMenu mainMenu;

        /// <summary>
        /// The main entry point of the client.
        /// </summary>
        /// <param name="args">
        ///     The command-line arguments passed to the client.
        /// </param>
        private static void Main(string[] args)
        {
            // Initialize input
            input = new ConsoleInput();
            input.StartReading();

            // Initialize output
            ConsoleRenderer r = new ConsoleRenderer();

            // Main Menu
            mainMenu = new ClientMenu(
                new[] { r },
                input,
                new TcpClient(/*new IPEndPoint(IPAddress.Any, 12345)*/)
                {
                    ReceiveBufferSize = ushort.MaxValue * 16,
                    SendBufferSize = ushort.MaxValue * 16,
                    SendTimeout = 30
                });

            mainMenu.StartWork();
            mainMenu.Show();

            // Wait until closed
            mainMenu.WaitForClose();
            Environment.Exit(0);
        }
    }
}
