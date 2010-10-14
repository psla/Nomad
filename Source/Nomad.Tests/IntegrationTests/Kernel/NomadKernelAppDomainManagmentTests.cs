using System;
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
        private const string AssemblyPath = @"SimplestModulePossible1";

        private string _assemblyFullPath;
        private NomadKernel _nomadKernel;


        [SetUp]
        public void set_up()
        {
            //Use default configuration if not specified otherwise.
            _nomadKernel = new NomadKernel();

            _assemblyFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AssemblyPath);
        }


        [Test]
        public void loading_module_into_module_appdomain()
        {
            var expectedModuleInfos = new[]
                                          {
                                              //Set Up modules to be loaded.
                                              new ModuleInfo(_assemblyFullPath),
                                          };

            var moduleDiscoveryMock = new Mock<IModuleDiscovery>(MockBehavior.Loose);
            moduleDiscoveryMock.Setup(x => x.GetModules())
                .Returns(expectedModuleInfos);

            _nomadKernel.ModuleAppDomain.AssemblyLoad +=
                (sender, args) => Assert.AreEqual(AssemblyFullName,
                                                  args.LoadedAssembly.
                                                      FullName,
                                                  "The module has not been loaded into Module AppDomain");

            _nomadKernel.ModuleManager.LoadModules(moduleDiscoveryMock.Object);

            foreach (Assembly kernelAsm in _nomadKernel.KernelAppDomain.GetAssemblies())
            {
                Assert.AreNotEqual(AssemblyFullName, kernelAsm,
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
            //TODO: wrtie 2
        }


        [Test]
        public void unloading_modules_upon_request()
        {
            //TODO: write 3
        }
    }
}