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
    using System.Reflection;
    using Core.Component;
    using System.Net.Sockets;
    using System.Net;
    using System.Threading;
    using System.Runtime.Serialization.Formatters.Binary;
    using ClientServerCommunication;
    using ClientJobExecutor;

    /// <summary>
    /// Provides methods for executing a component.
    /// </summary>
    public static class JobExecutor
    {
        private static int lastPort = 15000;

        /// <summary>
        /// Gets the 
        /// </summary>
        public static Dictionary<string, Assembly> Data { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
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
                using(FileStream fs = new FileStream("temp\\" + dll, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.Read(dllrd, 0, (int)dlli.Length);
                }

                ExecuteRequest arg = new ExecuteRequest(dllrd, args.ToArray());
                MemoryStream ms = new MemoryStream();

                f.Serialize(ms, arg);
                long length = ms.Length;

                byte b1, b100, b10000, b1000000;

                b1 = (byte)(length % 0x100);
                b100 = (byte)((length / 0x100) % 0x100);
                b10000 = (byte)((length / 0x10000) % 0x100);
                b1000000 = (byte)((length / 0x1000000) % 0x100);

                List<byte> send = new List<byte>(new byte[] { 0, 0, 0, 0, b1, b100, b10000, b1000000, (byte)arg.MessageType });
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
        /// 
        /// </summary>
        public static string StorePath
        { 
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "temp");
            }
        }

        /// <summary>
        /// 
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
                using (FileStream fs = new FileStream(dll, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] rd = new byte[len];
                    fs.Read(rd, 0, (int)len);
                    Data.Add(fn, Assembly.Load(rd));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dll"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
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
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dll"></param>
        /// <returns></returns>
        private static IComponent ReadComponentInfoFromDll(Assembly dll)
        {
            Type componentInfo = null;

            var result = from type in dll.GetTypes()
                         where typeof(IComponent).IsAssignableFrom(type)
                         select type;

            componentInfo = result.Single();

            IComponent comp = (IComponent)Activator.CreateInstance(componentInfo);
            return comp;
        }
    }
}
