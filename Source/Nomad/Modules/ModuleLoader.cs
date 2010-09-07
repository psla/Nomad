using System;
using System.Reflection;
using System.Linq;

namespace Nomad.Modules
{
    public class ModuleLoader
    {
        public void LoadModuleFromFile(string s)
        {
            var assembly = Assembly.LoadFile(s);
            var bootstraperTypes = from type in assembly.GetTypes()
                                   where type.GetInterfaces().Contains(typeof (IModuleBootstraper))
                                   select type;

            var bootstraperType = bootstraperTypes.SingleOrDefault();
            var bootstraper = (IModuleBootstraper)Activator.CreateInstance(bootstraperType);
            bootstraper.Initialize();
        }
    }
}