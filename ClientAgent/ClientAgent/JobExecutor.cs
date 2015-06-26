// --------------------------------------------------------------
// <copyright file="JobExecutor.cs" company="David Eiwen">
// (c) by David Eiwen. All Rights reserved.
// </copyright>
// <summary>
// This file contains the <see cref="JobExecutor"/> class.
// </summary>
// <author>
// David Eiwen
// </author>
// --------------------------------------------------------------

namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using ClientJobExecutor;
    using ClientServerCommunication;
    using Core.Component;

    /// <summary>
    /// Provides methods for executing a component.
    /// </summary>
    public static class JobExecutor
    {
        /// <summary>
        /// Contains the next port for the executor TCP connection.
        /// </summary>
        private static int lastPort = 15000;

        /// <summary>
        /// Gets the assemblies that are stored locally.
        /// </summary>
        /// <value>
        ///     Contains the assemblies that are stored locally.
        /// </value>
        public static Dictionary<string, Assembly> Data { get; private set; }

        /// <summary>
        /// Gets the path of the temporary directory.
        /// </summary>
        /// <value>
        ///     Contains the path of the temporary directory.
        /// </value>
        public static string StorePath
        { 
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "temp");
            }
        }

        /// <summary>
        /// Executes a specific component.
        /// </summary>
        /// <param name="dll">
        ///     The file name of the component to execute.
        /// </param>
        /// <param name="args">
        ///     The arguments passed to the component.
        /// </param>
        /// <returns>
        ///     Returns an instance of <see cref="IEnumerable"/>&#x3c;object&#x3e; containing the result.
        /// </returns>
        public static IEnumerable<object> Execute(string dll, IEnumerable<object> args)
        {
            Process client = new Process();
            client.StartInfo.FileName = "ClientJobExecutor.exe";

            int port = 0;
            TcpListener l;
            do
            {
                try
                {
                    port = lastPort;
                    l = new TcpListener(new IPEndPoint(IPAddress.Any, lastPort++));
                    l.Start();
                    if (lastPort >= ushort.MaxValue)
                    {
                        lastPort = 15000;
                    }

                    break;
                }
                catch
                {
                    Thread.Sleep(20);
                }
            }
            while (true);

            if (args == null)
            {
                args = new object[0];
            }

            client.StartInfo.Arguments = port.ToString();
            client.StartInfo.CreateNoWindow = false;
            client.Start();

            if (!client.HasExited)
            {
                TcpClient cl = l.AcceptTcpClient();
                cl.SendBufferSize = ushort.MaxValue * 16;
                cl.ReceiveBufferSize = ushort.MaxValue * 16;
                cl.ReceiveTimeout = 30;

                BinaryFormatter f = new BinaryFormatter();

                FileInfo dlli = new FileInfo("temp\\" + dll);
                byte[] dllrd = new byte[dlli.Length];
                using (FileStream fs = new FileStream("temp\\" + dll, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.Read(dllrd, 0, (int)dlli.Length);
                }

                ExecuteRequest arg = new ExecuteRequest(dllrd, args.ToArray());

                byte[] send = Client.SerializeMessage(arg);

                NetworkStream nstr = cl.GetStream();
                nstr.Write(send, 0, send.Length);

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
                            if (rcv.MessageType == StatusCode.ExecuteJob)
                            {
                                ExecuteResponse rsp = rcv as ExecuteResponse;
                                l.Stop();
                                return rsp.Result;
                            }
                        }

                        break;
                    }

                    Thread.Sleep(10);
                }

                client.WaitForExit();
            }

            l.Stop();

            return new object[] { new InvalidOperationException() };
        }

        /// <summary>
        /// Reads all the component files stored in the temporary directory.
        /// </summary>
        public static void GetAssemblies()
        {
            Data = new Dictionary<string, Assembly>();

            foreach (string dll in Directory.GetFiles(StorePath, "*dll"))
            {
                string fn = Path.GetFileName(dll);
                if (fn == "Core.Component.dll")
                {
                    continue;
                }

                long len = new FileInfo(dll).Length;
                try
                {
                    using (FileStream fs = new FileStream(dll, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        byte[] rd = new byte[len];
                        fs.Read(rd, 0, (int)len);
                        Data.Add(fn, Assembly.Load(rd));
                    }
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Stores a component temporarily.
        /// </summary>
        /// <param name="dll">
        ///     The byte array of the component file to store.
        /// </param>
        /// <param name="filename">
        ///     The file name where the component shall be stored.
        /// </param>
        /// <returns>
        ///     Returns a value indicating whether the component was successfully stored.
        /// </returns>
        public static bool StoreComponent(byte[] dll, string filename)
        {
            try
            {
                using (FileStream fs = new System.IO.FileStream(
                    filename,
                    System.IO.FileMode.Create,
                    System.IO.FileAccess.Write))
                {
                    fs.Write(dll, 0, dll.Length);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
