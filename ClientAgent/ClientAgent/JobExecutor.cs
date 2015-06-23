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
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Core.Component;

    /// <summary>
    /// Provides methods for executing a component.
    /// </summary>
    public static class JobExecutor
    {
        /// <summary>
        /// Gets the 
        /// </summary>
        public static Dictionary<string, Assembly> Data { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IEnumerable<object> Execute(Assembly dll, IEnumerable<object> args)
        {
            IComponent comp = ReadComponentInfoFromDll(dll);
            if (args == null)
            {
                args = new object[0];
            }

            return comp.Evaluate(args);
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
                    StorePath + "\\" + filename + ".dll",
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
