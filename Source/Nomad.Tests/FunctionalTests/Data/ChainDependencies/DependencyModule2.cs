using System;
using System.IO;
using DependencyModule;

namespace DependencyModuleNamespace2
{
    public class DependencyModule2 : Nomad.Modules.IModuleBootstraper
    {
        public void OnLoad()
        {
            DependencyModule1 module1 = new DependencyModule1();
            module1.Execute();

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