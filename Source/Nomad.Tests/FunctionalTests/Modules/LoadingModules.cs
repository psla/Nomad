using System;
using System.IO;
using System.Reflection;
using Nomad.Modules;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Modules
{
    [FunctionalTests]
    public class LoadingModules
    {
        [Test]
        public void loads_one_module_and_executes_its_bootstraper()
        {
            var libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           @"Modules\SimplestModulePossible1.dll");
            var moduleLoader = new ModuleLoader();
            moduleLoader.LoadModuleFromFile(libraryPath);
            var registeredModules = LoadedModulesRegistry.GetRegisteredModules();
            Assert.AreEqual(1, registeredModules.Count);
        }
    }
}