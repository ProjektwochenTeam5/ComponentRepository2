// ----------------------------------------------------------------------- 
// <copyright file="PRogram.cs" company="Gunter Wiesinger"> 
// Copyright (c) Gunter Wiesinger. All rights reserved. 
// </copyright> 
// <summary>This application is just for testing purposes.</summary> 
// <author>Gunter Wiesinger/Auto generated</author> 
// -----------------------------------------------------------------------
namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Threading.Tasks;
    using ClientServerCommunication;
    using ConsoleGUI.IO;
    using ConsoleGUI.Controls;
    using System.Collections.ObjectModel;

    public class Program
    {
        public static void Main(string[] args)
        {
            GUI.Do();
            //ConsoleRenderer r = new ConsoleRenderer();
            //ConsoleInput i = new ConsoleInput();
            //i.StartReading();

            //Menu m = new Menu(new Collection<IRenderer>() { r }, i);
            //MenuButton btn = new MenuButton() { Enabled = true, LinkedKey = ConsoleKey.F1, Text = "F1 Quit", Visible = true };
            //btn.ButtonKeyPressed += btn_ButtonKeyPressed;
            //m.Buttons.Add(btn);
            //m.Visible = true;
            //m.Focused = true;
            //m.Draw();
            //i.InputReceived += i_InputReceived;


            RecBroadcast b = new RecBroadcast();
            Task udpTask = new Task(new Action(() => b.Recieve()));
            udpTask.Start();

            TCPServerManager tcpservermanager = new TCPServerManager();
            Task tcpTask = new Task(() => tcpservermanager.RunMyServer());
            tcpTask.Start();

            //ServerBroadcast serverBroadcast = new ServerBroadcast();
            //Task serverBroadcastTask = new Task(() => serverBroadcast.SendBroadcast());
            //serverBroadcastTask.Start();

            //ServerReceiver serverReceiver = new ServerReceiver();
            //Task serverReceiverTask = new Task(() => serverReceiver.StartReceiving());
            //serverReceiverTask.Start();
           
            //m.WaitForClose();
            Console.ReadLine();
        }

        static void i_InputReceived(object sender, InputReceivedEventArgs e)
        {
            Console.Write(e.ReceivedKey.KeyChar);
        }

        static void btn_ButtonKeyPressed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
