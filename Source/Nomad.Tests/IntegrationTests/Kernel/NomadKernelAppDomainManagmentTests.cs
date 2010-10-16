using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Moq;
using Nomad.Core;
using Nomad.Modules;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Kernel
{
    /// <summary>
    /// Checks about AppDomain proper implementation within Nomad.Kernel.
    /// </summary>
    [IntegrationTests]
    public class NomadKernelAppDomainManagmentTests
    {
        private const string AssemblyFullName = @"SimplestModulePossible1";
        private const string AssemblyFullName2 = @"SimplestModulePossible2";

        private const string AssemblyPath = @"SimplestModulePossible1";
        private const string AssemblyPath2 = @"SimplestModulePossible2";

        private string _assemblyFullPath;
        private string _assemblyFullPath2;
        private NomadKernel _nomadKernel;
        private Mock<IModuleDiscovery> _moduleDiscoveryMock;

        private void SetUpModuleDiscovery(IEnumerable<ModuleInfo> moduleInfos)
        {
            _moduleDiscoveryMock = new Mock<IModuleDiscovery>(MockBehavior.Loose);
            _moduleDiscoveryMock.Setup(x => x.GetModules())
                .Returns(moduleInfos);
        }

        [SetUp]
        public void set_up()
        {

            //Use default configuration if not specified otherwise.
            var kernelAppDomain = AppDomain.CreateDomain("Kernel AppDomain");
            _nomadKernel = new NomadKernel();
            _assemblyFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AssemblyPath);
            _assemblyFullPath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AssemblyPath2);

        }


        [Test]
        public void loading_module_into_module_appdomain()
        {
            var expectedModuleInfos = new[]
                                          {
                                              //Set Up modules to be loaded.
                                              new ModuleInfo(_assemblyFullPath),
                                          };

            SetUpModuleDiscovery(expectedModuleInfos);

            _nomadKernel.ModuleAppDomain.AssemblyLoad +=
                (sender, args) => Assert.AreEqual(AssemblyFullName,
                                                  args.LoadedAssembly.
                                                      FullName,
                                                  "The module has not been loaded into Module AppDomain");

            _nomadKernel.ModuleManager.LoadModules(_moduleDiscoveryMock.Object);

            //Check for not loading asm into kernel appDomain
            foreach (Assembly kernelAsm in _nomadKernel.KernelAppDomain.GetAssemblies())
            {
                Assert.AreNotEqual(AssemblyFullName, kernelAsm.FullName,
                                   "The module assembly has been loaded into KernelAppDomain.");
            }
        }


        [Test]
        public void verifing_starting_appdomain_to_have_not_module_loading_implementation_loaded()
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Assert.AreNotEqual(AssemblyFullName, asm.FullName);
            }
        }


        [Test]
        public void loading_more_than_one_module_into_module_appdomain()
        {
            
            var expectedModuleInfos = new[]
                                          {
                                              //Set Up modules to be loaded.
                                              new ModuleInfo(_assemblyFullPath),
                                              new ModuleInfo(_assemblyFullPath2), 
                                          };

            SetUpModuleDiscovery(expectedModuleInfos);

            _nomadKernel.ModuleAppDomain.AssemblyLoad += (sender, args) =>
                                                             {
                                                                 //Check for one on another
                                                                 Assert.That(
                                                                     args.LoadedAssembly.FullName.Equals(AssemblyFullName) 
                                                                     ||
                                                                     args.LoadedAssembly.FullName.Equals(AssemblyFullName2)
                                                                     );
                                                             };

            _nomadKernel.ModuleManager.LoadModules(_moduleDiscoveryMock.Object);


            var firstLoaded = false;
            var secondLoaded = false;

            //Check that all were loaded
            foreach (var moduleAsm in _nomadKernel.ModuleAppDomain.GetAssemblies())
            {
                if (moduleAsm.FullName.Equals(AssemblyFullName))
                    firstLoaded = true;
                if (moduleAsm.FullName.Equals(AssemblyPath2))
                    secondLoaded = true;
            }
            Assert.IsTrue(firstLoaded && secondLoaded, "One of the modules has not been loaded");

            //Check for not loading asm into kernel appDomain);
            foreach (Assembly kernelAsm in _nomadKernel.KernelAppDomain.GetAssemblies())
            {
                Assert.AreNotEqual(AssemblyFullName, kernelAsm.FullName,
                                   "The module assembly 1 has been loaded into KernelAppDomain.");
                Assert.AreNotEqual(AssemblyFullName2, kernelAsm.FullName,
                                   "The module assembly 2 has been loaded into KernelAppDomain.");
            }
        }


        [Test]
        public void unloading_modules_upon_request()
        {
            var expectedModuleInfos = new[]
                                          {
                                              //Set Up modules to be loaded.
                                              new ModuleInfo(_assemblyFullPath),
                                              new ModuleInfo(_assemblyFullPath2), 
                                          };

            SetUpModuleDiscovery(expectedModuleInfos);

            _nomadKernel.ModuleManager.LoadModules(_moduleDiscoveryMock.Object);

            //TODO: check if this API is good enough
            _nomadKernel.UnloadModules();

            //Aseert modules unloaded

            foreach (var moduleAsm in _nomadKernel.ModuleAppDomain.GetAssemblies())
            {
                Assert.AreNotEqual(AssemblyFullName, moduleAsm.FullName,
                                  "The module assembly 1 has been loaded into KernelAppDomain.");
                Assert.AreNotEqual(AssemblyFullName2, moduleAsm.FullName,
                                   "The module assembly 2 has been loaded into KernelAppDomain.");
            }

        }
    }
}