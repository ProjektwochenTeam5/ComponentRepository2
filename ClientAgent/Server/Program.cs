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
    using System.IO;
    using Core.Component;

    public class Program
    {
        public static void Main(string[] args)
        {
            RecBroadcast b = new RecBroadcast();
            Task udpTask = new Task(new Action(() => b.Recieve()));
            udpTask.Start();

            TCPServerManager tcpservermanager = new TCPServerManager();
            Task tcpTask = new Task(() => tcpservermanager.RunMyServer());
            tcpTask.Start();


            Console.ReadLine();
        }
    }
}
