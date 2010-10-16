using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
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
    public class NomadKernelAppDomainManagmentTests : MarshalByRefObject
    {
        private const string AssemblyFullName = @"SimplestModulePossible1";
        private const string AssemblyFullName2 = @"SimplestModulePossible2";

        private const string AssemblyPath =  @"Modules\Simple\SimplestModulePossible1.dll";
        private const string AssemblyPath2 = @"Modules\Simple\SimplestModulePossible2.dll";

        private string _assemblyFullPath;
        private string _assemblyFullPath2;
        private NomadKernel _nomadKernel;
        private Mock<IModuleDiscovery> _moduleDiscoveryMock;
        private AppDomain _testAppDomain;


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
            _assemblyFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AssemblyPath);
            _assemblyFullPath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AssemblyPath2);
            _testAppDomain = AppDomain.CreateDomain("Kernel test domain", new Evidence(AppDomain.CurrentDomain.Evidence), AppDomain.CurrentDomain.BaseDirectory, ".", false);
        }

        [Test]
        public void loading_module_into_module_appdomain()
        {
            _testAppDomain.DoCallBack(loading_module_into_module_appdomain_callback);
        }


        private void loading_module_into_module_appdomain_callback()
        {
            _nomadKernel = new NomadKernel();

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
            _testAppDomain.DoCallBack(verifing_starting_appdomain_to_have_not_module_loading_implementation_loaded_callback);   
        }


        private void verifing_starting_appdomain_to_have_not_module_loading_implementation_loaded_callback()
        {
            _nomadKernel = new NomadKernel();

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Assert.AreNotEqual(AssemblyFullName, asm.FullName);
            }
        }


        [Test]
        public void loading_more_than_one_module_into_module_appdomain()
        {
            _testAppDomain.DoCallBack(loading_more_than_one_module_into_module_appdomain_callback);
        }


        private void loading_more_than_one_module_into_module_appdomain_callback()
        {
            _nomadKernel = new NomadKernel();

            var expectedModuleInfos = new[]
                                          {
                                              //Set Up modules to be loaded.
                                              new ModuleInfo(_assemblyFullPath),
                                              new ModuleInfo(_assemblyFullPath2), 
                                          };

            SetUpModuleDiscovery(expectedModuleInfos);

            _nomadKernel.ModuleAppDomain.AssemblyLoad += (sender, args) =>
                                                             {
                                                                 //Check for loading modules
                                                                 Assert.That(
                                                                     args.LoadedAssembly.FullName.Equals(AssemblyFullName) 
                                                                     ||
                                                                     args.LoadedAssembly.FullName.Equals(AssemblyFullName2)
                                                                     );
                                                             };

            _nomadKernel.ModuleManager.LoadModules(_moduleDiscoveryMock.Object);

            //Check for not loading asm into kernel appDomain);););
            foreach (Assembly kernelAsm in _nomadKernel.KernelAppDomain.ReflectionOnlyGetAssemblies())
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
            _testAppDomain.DoCallBack(unloading_modules_upon_request_callback);
        }


        private void unloading_modules_upon_request_callback()
        {
            _nomadKernel = new NomadKernel();

            var expectedModuleInfos = new[]
                                          {
                                              //Set Up modules to be loaded.
                                              new ModuleInfo(_assemblyFullPath),
                                              new ModuleInfo(_assemblyFullPath2), 
                                          };

            SetUpModuleDiscovery(expectedModuleInfos);

            _nomadKernel.ModuleManager.LoadModules(_moduleDiscoveryMock.Object);

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