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
        private ModuleLoader _moduleLoader;


        [SetUp]
        public void Setup()
        {
            LoadedModulesRegistry.Clear();
            _moduleLoader = new ModuleLoader();
        }
        [Test]
        public void loads_one_module_and_executes_its_bootstraper()
        {
            var libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           @"Modules\Simple\SimplestModulePossible1.dll");
            _moduleLoader.LoadModuleFromFile(libraryPath);
            var registeredModules = LoadedModulesRegistry.GetRegisteredModules();
            Assert.AreEqual(1, registeredModules.Count);
        }

        [Test]
        public void loads_all_modules_and_executes_their_bootstrapers()
        {
            var libraryPaths = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                   @"Modules\Simple\");
            _moduleLoader.LoadModulesFromDirectory(libraryPaths);
            var registeredModules = LoadedModulesRegistry.GetRegisteredModules();
            Assert.AreEqual(2, registeredModules.Count);
        }

        [Test]
        public void loads_all_modules_even_if_there_is_incorrect_file()
        {
            var incorrectFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                     @"Modules\Simple\bzdurnyplikk.txt");
            try
            {
                var libraryPaths = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                @"Modules\Simple\");
                
                File.WriteAllText(incorrectFilePath, @"bzdura");


                _moduleLoader.LoadModulesFromDirectory(libraryPaths);
                var registeredModules = LoadedModulesRegistry.GetRegisteredModules();
                Assert.AreEqual(2, registeredModules.Count);
            }
            finally
            {
                File.Delete(incorrectFilePath);
            }

        }
    }
}