using System;
using System.IO;

namespace DependencyModuleNamespace2
{
    public class DependencyModule2 : Nomad.Modules.IModuleBootstraper
    {
        public void OnLoad()
        {
            Nomad.Tests.FunctionalTests.Modules.LoadedModulesRegistry.Register(typeof(DependencyModule2));
        }

        public void OnUnLoad()
        {
            // nothing
        }

        public void Execute()
        {
            ;
        }
    }

}