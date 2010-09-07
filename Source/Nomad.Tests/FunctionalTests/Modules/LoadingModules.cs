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
        [SetUp]
        public void Setup()
        {
            LoadedModulesRegistry.Clear();
        }
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

        [Test]
        public void loads_all_modules_and_executes_their_bootstrapers()
        {
            var libraryPaths = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                   @"Modules");
            var moduleLoader = new ModuleLoader();
            moduleLoader.LoadModulesFromDirectory(libraryPaths);
            var registeredModules = LoadedModulesRegistry.GetRegisteredModules();
            Assert.AreEqual(2, registeredModules.Count);
        }
    }
}