using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Nomad.Modules
{
    /// <summary>
    /// Module loading logic. 
    /// </summary>
    public class ModuleLoader
    {
        /// <summary>
        /// Loades module from file.
        /// </summary>
        /// <param name="s">Path to module</param>
        public void LoadModuleFromFile(string s)
        {
            try
            {
                var assembly = Assembly.LoadFile(s);
                var bootstraperTypes = from type in assembly.GetTypes()
                                       where
                                           type.GetInterfaces().Contains(typeof (IModuleBootstraper))
                                       select type;

                var bootstraperType = bootstraperTypes.SingleOrDefault();
                var bootstraper = (IModuleBootstraper) Activator.CreateInstance(bootstraperType);
                bootstraper.Initialize();
            }
            catch(Exception e)
            {
                //_logger.WarnException("Couldn't load assembly", e);
            }
        }

        /// <summary>
        /// Loades all modules from provided directory path.
        /// </summary>
        /// <param name="libraryPaths">Path to directory</param>
        public void LoadModulesFromDirectory(string libraryPaths)
        {
            var files = Directory.GetFiles(libraryPaths);
            foreach (var file in files)
            {
                this.LoadModuleFromFile(file);
            }
        }
    }
}