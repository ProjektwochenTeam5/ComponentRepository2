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
            Task t = new Task(new Action(() => b.Recieve()));
            t.Start();
        }
    }
}
