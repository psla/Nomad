using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using Moq;
using Nomad.Core;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Kernel
{
    /// <summary>
    /// Checks about AppDomain proper implementation within Nomad.Kernel.
    /// </summary>
    /// <remarks>
    ///     Callback due to some issues with remoting automatic serialization features needs to be static.
    /// </remarks>
    [FunctionalTests]
    public class NomadKernelAppDomainManagmentTests : MarshalByRefObject
    {
        private const string AssemblyFullName = @"SimplestModulePossible1";
        private const string AssemblyFullName2 = @"SimplestModulePossible2";

        private const string AssemblyPath = @"Modules\Simple\SimplestModulePossible1.dll";
        private const string AssemblyPath2 = @"Modules\Simple\SimplestModulePossible2.dll";

        private static string _assemblyFullPath;
        private static string _assemblyFullPath2;
        private static Mock<IModuleDiscovery> _moduleDiscoveryMock;
        private static NomadKernel _nomadKernel;
        private AppDomain _testAppDomain;
        private static NomadConfiguration _configuration;


        private static void SetUpModuleDiscovery(IEnumerable<ModuleInfo> moduleInfos)
        {
            _moduleDiscoveryMock = new Mock<IModuleDiscovery>(MockBehavior.Loose);
            _moduleDiscoveryMock.Setup(x => x.GetModules())
                .Returns(moduleInfos);
        }


        [SetUp]
        public void set_up()
        {
            _testAppDomain = AppDomain.CreateDomain("Kernel test domain",
                                                    new Evidence(AppDomain.CurrentDomain.Evidence),
                                                    AppDomain.CurrentDomain.BaseDirectory, 
                                                    ".",
                                                    false);
        }

        private static void SetUpInDomain()
        {
            _assemblyFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AssemblyPath);
            _assemblyFullPath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AssemblyPath2);

            _configuration = NomadConfiguration.Default;
            var dependencyMock = new Mock<IDependencyChecker>(MockBehavior.Loose);
            dependencyMock.Setup(x => x.SortModules(It.IsAny<IEnumerable<ModuleInfo>>()))
                .Returns<IEnumerable<ModuleInfo>>(e => e);
            _configuration.DependencyChecker = dependencyMock.Object;
        }

        [Test]
        public void loading_module_into_module_appdomain()
        {
            _testAppDomain.DoCallBack(loading_module_into_module_appdomain_callback);
        }


        private static void loading_module_into_module_appdomain_callback()
        {
            SetUpInDomain();
           
            _nomadKernel = new NomadKernel(_configuration);

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

            _nomadKernel.ModuleAppDomain.UnhandledException +=
                (sender, args) => Assert.Fail("Exception has been thrown" + args.ToString());

            _nomadKernel.LoadModules(_moduleDiscoveryMock.Object);

            //Check for not loading assembly into kernel appDomain
            foreach (Assembly kernelAsm in _nomadKernel.KernelAppDomain.GetAssemblies())
            {
                Assert.AreNotEqual(AssemblyFullName, kernelAsm.FullName,
                                   "The module assembly has been loaded into KernelAppDomain.");
            }
        }


        [Test]
        public void verifing_starting_appdomain_to_have_not_module_loading_implementation_loaded()
        {
            _testAppDomain.DoCallBack(
                verifing_starting_appdomain_to_have_not_module_loading_implementation_loaded_callback);
        }


        private static void
            verifing_starting_appdomain_to_have_not_module_loading_implementation_loaded_callback()
        {
            SetUpInDomain();
            _nomadKernel = new NomadKernel(_configuration);

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


        private static void loading_more_than_one_module_into_module_appdomain_callback()
        {
            SetUpInDomain();
            _nomadKernel = new NomadKernel(_configuration);

            var expectedModuleInfos = new[]
                                          {
                                              //Set Up modules to be loaded.
                                              new ModuleInfo(_assemblyFullPath),
                                              new ModuleInfo(_assemblyFullPath2),
                                          };

            SetUpModuleDiscovery(expectedModuleInfos);

            _nomadKernel.ModuleAppDomain.AssemblyLoad += (sender, args) => Assert.That(
                args.LoadedAssembly.FullName.
                    Equals(AssemblyFullName)
                ||
                args.LoadedAssembly.FullName.
                    Equals(AssemblyFullName2)
                                                                               );

            _nomadKernel.LoadModules(_moduleDiscoveryMock.Object);

            //Check for not loading assembly into kernel appDomain);););
            foreach (
                Assembly kernelAsm in _nomadKernel.KernelAppDomain.ReflectionOnlyGetAssemblies())
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
            //_testAppDomain.DoCallBack(unloading_modules_upon_request_callback);
            unloading_modules_upon_request_callback();
        }


        private static void unloading_modules_upon_request_callback()
        {
            SetUpInDomain();
            _nomadKernel = new NomadKernel(_configuration);

            var expectedModuleInfos = new[]
                                          {
                                              //Set Up modules to be loaded.
                                              new ModuleInfo(_assemblyFullPath),
                                              new ModuleInfo(_assemblyFullPath2),
                                          };

            SetUpModuleDiscovery(expectedModuleInfos);

            _nomadKernel.LoadModules(_moduleDiscoveryMock.Object);

            var moduleAppDomain = _nomadKernel.ModuleAppDomain;

            _nomadKernel.UnloadModules();

            Assert.AreNotSame(moduleAppDomain,_nomadKernel.ModuleAppDomain);
        }
    }
}