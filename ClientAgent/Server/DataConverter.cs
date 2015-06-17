using ClientServerCommunication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class DataConverter
    {
        public static byte[] ConvertMessageToByteArray(int t, byte[] data)
        {
            byte[] check = new byte[] { 0, 0, 0, 0 };
            byte[] length = new byte[4];
            byte[] type = new byte[1];

            length = BitConverter.GetBytes(data.Length);
            type = BitConverter.GetBytes(t);

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
    }
}
