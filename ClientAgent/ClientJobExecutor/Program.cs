
namespace ClientJobExecutor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Component;
    using ClientServerCommunication;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int port;
            if(!int.TryParse(args[0], out port))
            {
                return;
            }

            TcpClient cl = new TcpClient();
            cl.Connect(new IPEndPoint(IPAddress.Loopback, port));
            NetworkStream str = cl.GetStream();

            while (true)
            {
                if (str.DataAvailable)
                {
                    byte[] hdr = new byte[9];
                    str.Read(hdr, 0, 9);

                    uint length = 0;
                    StatusCode sc;

                    if (hdr.Length != 9)
                    {
                        continue;
                    }

                    if (hdr[0] != 0 || hdr[1] != 0 || hdr[2] != 0 || hdr[3] != 0)
                    {
                        continue;
                    }

                    length = (uint)(hdr[4] + (hdr[5] * 0x100) + (hdr[6] * 0x10000) + (hdr[7] * 0x1000000));
                    sc = (StatusCode)hdr[8];

                    BinaryFormatter f = new BinaryFormatter();
                    byte[] rd = new byte[length];
                    str.Read(rd, 0, (int)length);

                    using(MemoryStream ms = new MemoryStream(rd))
                    {
                        Message m = (Message)f.Deserialize(ms);
                        if (m.MessageType == StatusCode.ExecuteJob)
                        {
                            ExecuteRequest rq = m as ExecuteRequest;

                            Assembly a = Assembly.Load(rq.Assembly);

                            Type componentInfo = null;

                            var result = from type in a.GetTypes()
                                         where typeof(IComponent).IsAssignableFrom(type)
                                         select type;

                            componentInfo = result.Single();
                            Console.Title = a.GetName().Name;

                            IComponent comp = (IComponent)Activator.CreateInstance(componentInfo);
                            object[] res = comp.Evaluate(rq.InputData).ToArray();

                            ExecuteResponse rsp = new ExecuteResponse(res, false);

                            using (MemoryStream resstr = new MemoryStream())
                            {
                                f.Serialize(resstr, rsp);
                                List<byte> resp = new List<byte>();

                                long len = resstr.Length;
                                byte b1, b100, b10000, b1000000;

                                b1 = (byte)(len % 0x100);
                                b100 = (byte)((len / 0x100) % 0x100);
                                b10000 = (byte)((len / 0x10000) % 0x100);
                                b1000000 = (byte)((len / 0x1000000) % 0x100);

                                resp.AddRange(new byte[] { 1, 1, 1, 1, b1, b100, b10000, b1000000, (byte)rsp.MessageType });
                                resp.AddRange(resstr.ToArray());
                                str.Write(resp.ToArray(), 0, resp.Count);
                                break;
                            }
                        }
                    }

                    break;
                }

                Thread.Sleep(5);
            }

            cl.Close();
            Environment.Exit(0);
        }
    }
}
