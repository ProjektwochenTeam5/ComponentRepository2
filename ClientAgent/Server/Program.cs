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

    public class Program
    {
        public static void Main(string[] args)
        {
            RecBroadcast b = new RecBroadcast();
            Task udpTask = new Task(new Action(() => b.Recieve()));
            udpTask.Start();

            TCPServer tcp = new TCPServer();
            tcp.OnMessageRecieved += tcp_OnMessageRecieved;
            Task tcpTask = new Task(new Action(() => tcp.Run()));
            tcpTask.Start();

            Console.ReadKey();
        }

        static void tcp_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch (e.MessageType)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
            }
        }
    }
}
