namespace Server
{
    using Core.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
    using Core.Component;

    public class DataBaseWrapper
    {
        public List<Assembly> Data { get; set; }

        public string StorePath { get { return Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName) + "\\Store"; } }

        public void GetAssemblies()
        {
            this.Data = new List<Assembly>();

            foreach (var dll in Directory.GetFiles(this.StorePath, "*dll"))
            {
                this.Data.Add(Assembly.LoadFile(dll));
            }

        }

        public IComponent ReadComponentInfoFormDll(Assembly dll)
        {
            Type componentInfo = null;

            var result = from type in dll.GetTypes()
                          where typeof(IComponent).IsAssignableFrom(type)
                          select type;

            componentInfo = result.Single();

            IComponent comp = (IComponent)Activator.CreateInstance(componentInfo);
            return comp;
        }

        public bool StoreComponent(byte[] dll, string filename)
        {
            try
            {
                System.IO.FileStream fs=
                   new System.IO.FileStream(this.StorePath + "\\" + filename + ".dll", System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);

                fs.Write(dll, 0, dll.Length);

                // close file stream
                fs.Close();

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
}
