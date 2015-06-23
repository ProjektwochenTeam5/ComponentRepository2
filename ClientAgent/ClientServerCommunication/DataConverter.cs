using ClientServerCommunication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Core.Component;
using Core.Network;

namespace ClientServerCommunication
{
    public static class DataConverter
    {
        public static byte[] ConvertMessageToByteArray(byte t, byte[] data)
        {
            byte[] check = new byte[] { 1, 1, 1, 1 };
            byte[] length = new byte[4];
            byte[] type = new byte[] { t };

            length = BitConverter.GetBytes(data.Length);

            byte[] send = new byte[check.Length + length.Length + type.Length + data.Length];
            int index = 0;
            for (int i = 0; i < check.Length; i++)
            {
                send[index] = check[i];
                index++;
            }

            for (int i = 0; i < length.Length; i++)
            {
                send[index] = length[i];
                index++;
            }

            for (int i = 0; i < type.Length; i++)
            {
                send[index] = type[i];
                index++;
            }

            for (int i = 0; i < data.Length; i++)
            {
                send[index] = data[i];
                index++;
            }

            return send;
        }

        public static byte[] ConvertObjectToByteArray(Message m)
        {
            BinaryFormatter bf = new BinaryFormatter();

            byte[] serializedaccept = null;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, m);
                    serializedaccept = ms.ToArray();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return serializedaccept;
        }

        public static Message ConvertByteArrayToMessage(byte[] data)
        {
            Message m = null;

            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    m = (Message)bf.Deserialize(ms);
                    return m;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static Component MapIComponentToNetworkComponent(IComponent comp, Guid g)
        {

            Component c = new Component();
            c.IsAtomic = true;
            c.FriendlyName = comp.FriendlyName;
            c.ComponentGuid = g;

            if (comp.InputDescriptions != null)
            {
                List<string> inde = new List<string>();

                foreach (var item in comp.InputDescriptions)
                {
                    inde.Add(item);
                }
            }
            else
            {
                c.InputDescriptions = null;
            }

            if (comp.OutputDescriptions != null)
            {
                List<string> outde = new List<string>();

                foreach (var item in comp.OutputDescriptions)
                {
                    outde.Add(item);
                }
            }
            else
            {
                c.OutputDescriptions = null;
            }

            List<string> inhi = new List<string>();

            foreach (var item in comp.InputHints)
            {
                inhi.Add(item);
            }

            List<string> outhi = new List<string>();

            foreach (var item in comp.OutputHints)
            {
                outhi.Add(item);
                
            }

            c.InputHints = inhi;
            c.OutputHints = outhi;

            return c;
        }

        public static byte[] ConvertDllToByteArray(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static byte[] ConvertJsonToByteArray(MessageCode m, string json)
        {
            var jsonbytes = Encoding.ASCII.GetBytes(json);
            byte[] send = new byte[jsonbytes.Length + 5];
            send[0] = (byte)m;

            var length = BitConverter.GetBytes(jsonbytes.Length);
            for (int i = 1; i < 5; i++)
            {
                send[i] = length[i - 1];
            }

            for (int i = 5; i < send.Length; i++)
            {
                send[i] = jsonbytes[i];
            }

            return send;
        }
    }
}
