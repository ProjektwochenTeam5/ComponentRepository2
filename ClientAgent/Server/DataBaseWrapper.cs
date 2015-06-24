﻿namespace Server
{
    using Core.Network;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Component;

    public class DataBaseWrapper
    {
        public Dictionary<byte[], string> Data { get; set; }

        public object locker = new object();

        public string StorePath { get { return Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName) + "\\Store"; } }

        public void GetAssemblies()
        {
            lock (this.locker)
            {
                this.Data = new Dictionary<byte[], string>();

                foreach (string item in Directory.GetFiles(this.StorePath))
                {
                    if (Path.GetFileName(item) == "Core.Component.dll")
                    {
                        continue;
                    }

                    byte[] data = File.ReadAllBytes(item);
                    this.Data.Add(data, item);
                }
            }
        }

        public IComponent ReadComponentInfoFormDll(byte[] dll)
        {
            lock (this.locker)
            {
                Assembly a = Assembly.Load(dll);
                Type componentInfo = null;

                var result = from type in a.GetTypes()
                             where typeof(IComponent).IsAssignableFrom(type)
                             select type;


                componentInfo = result.Single();

                IComponent comp = (IComponent)Activator.CreateInstance(componentInfo);
                return comp;
            }
        }

        public bool StoreComponent(byte[] dll, string filename)
        {
            lock (this.locker)
            {
                try
                {

                    using (FileStream fs = new FileStream(this.StorePath + "\\" + filename + ".dll", System.IO.FileMode.Create,
                                                System.IO.FileAccess.Write, FileShare.Read))
                    {
                        fs.Write(dll, 0, dll.Length);

                    }

                    Console.WriteLine("--> " + filename + " Component succsessfully stored!");
                    return true;
                }
                catch (Exception e)
                {
                    // Error
                    Console.WriteLine("Exception caught in process: {0}",
                                      e.ToString());
                }

                // error occured, return false
                return false;
            }
        }

        public bool StoreComplexComponent(byte[] complexComponent, string filename)
        {
            using (FileStream fs = new FileStream(this.StorePath + "\\" + filename + ".dat", FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, complexComponent);
                    return true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to store complex component");
                    return false;
                }
            }
        }

        public byte[] GetComplexComponent(string filename)
        {
            using (FileStream fs = new FileStream(this.StorePath + "\\" + filename + ".dat", FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (byte[])formatter.Deserialize(fs);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not read complex component from file due to following reason: \n  " + e.Message);
                    return null;
                }
            }
        }
    }
}
