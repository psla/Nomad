using System;
using System.IO;
using System.Linq;
using Nomad.Core;
using Nomad.Exceptions;
using Nomad.KeysGenerator;
using Nomad.Messages;
using Nomad.Messages.Loading;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Tests.FunctionalTests.Fixtures;
using Nomad.Tests.FunctionalTests.Modules;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.Kernel
{
    [FunctionalTests]
    public class NomadKernelEventRaising : NomadKernelTestFixture
    {
        [Test]
        public void
            exception_during_module_loading_is_changed_into_event_visible_from_module_and_kernel()
        {
            // set up configuration of kernel with mocks
            NomadKernel kernel = SetupMockedKernel();
            
            // set up listener for kernel side
            bool hasBeenCalled = false;
            kernel.EventAggregator.Subscribe<NomadAllModulesLoadedMessage>(
                (message) => hasBeenCalled = true);

            //  compile module for event aggregation
            const string dir = @"Modules\Kernel\Event\";
            const string fileName = "EventAwareModule.dll";
            const string keyFile = @"newkey.xml";

            var compiler = new ModuleCompiler
                               {
                                   OutputDirectory =
                                       Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir)
                               };

            compiler.OutputName = Path.Combine(compiler.OutputDirectory, fileName);

            string modulePath = compiler.GenerateModuleFromCode(
                @"..\Source\Nomad.Tests\FunctionalTests\Data\Kernel\EventAwareModule.cs");

            KeysGeneratorProgram.Main(new[] {keyFile});
            compiler.GenerateManifestForModule(modulePath, keyFile);

            // set up module which subscribes for event
            IModuleDiscovery setUpDiscovery = SetUpDiscovery(new ModuleInfo(modulePath));
            kernel.LoadModules(setUpDiscovery);

            // set up the discovery mock
            IModuleDiscovery nonExistingDiscovery =
                SetUpDiscovery(new ModuleInfo("NonExistingModule.dll"));

            // perform test
           Assert.Throws<NomadCouldNotLoadModuleException>(
                () => kernel.LoadModules(nonExistingDiscovery),
                "Exception should  be thrown in kernel domain.");

            //verify the method being called in a module.
            var carrier = (MessageCarrier) kernel.ModuleAppDomain.CreateInstanceAndUnwrap(
                typeof (MessageCarrier).Assembly.FullName, typeof (MessageCarrier).FullName);

            Assert.AreEqual(new[] {"EventAwareModule"}, carrier.List.ToArray());
            Assert.IsTrue(hasBeenCalled);
        }


        private NomadKernel SetupMockedKernel()
        {
            NomadConfiguration configuration = NomadConfiguration.Default;
            configuration.ModuleFilter = _moduleFilterMock.Object;
            configuration.DependencyChecker = _dependencyCheckerMock.Object;

            return new NomadKernel(configuration);
        }


        [Test]
        public void event_after_successful_module_loading_is_published()
        {
            NomadKernel kernel = SetupMockedKernel();

            // subscribe for message in kernel
            bool hasBeenLoaded = false;
            kernel.EventAggregator.Subscribe<NomadAllModulesLoadedMessage>( (message) =>    
            {
                
                Assert.AreEqual(3,message.ModuleInfos.Count());
                hasBeenLoaded = true;                                                                      
            });

            //  compile module for event aggregation
            const string dir = @"Modules\Kernel\AllModulesLoadedAwarenessTestModules\";
            const string awareModuleName = "AllModulesLoadedEventAwareModule.dll";
            const string module1Name = "SimpleModule1.dll";
            const string module2Name = "SimpleModule2.dll";
            const string keyFile = @"newkey.xml";

            CompileSimpleModules(dir, keyFile, module1Name, module2Name);

            var compiler = new ModuleCompiler
                               {
                                   OutputDirectory =
                                       Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir)
                               };

            compiler.OutputName = Path.Combine(compiler.OutputDirectory, awareModuleName);

            string modulePath = compiler.GenerateModuleFromCode(
                @"..\Source\Nomad.Tests\FunctionalTests\Data\Kernel\AllModulesLoadedEventAwareModule.cs");

            KeysGeneratorProgram.Main(new[] {keyFile});
            compiler.GenerateManifestForModule(modulePath, keyFile);

            var directoryDiscovery = new DirectoryModuleDiscovery(dir, SearchOption.TopDirectoryOnly);

            // loading modules
            Assert.DoesNotThrow(() => kernel.LoadModules(directoryDiscovery));

            //verify the method being called in a module.
            var carrier = (MessageCarrier) kernel.ModuleAppDomain.CreateInstanceAndUnwrap(
                typeof (MessageCarrier).Assembly.FullName, typeof (MessageCarrier).FullName);

            Assert.AreEqual(new[] {"AllModulesLoadedEventAwareModule"}, carrier.List.ToArray());
            Assert.IsTrue(hasBeenLoaded);
        }


        [Test]
        public void event_all_modules_loaded_is_catched_upon_every_success()
        {
            NomadKernel kernel = SetupMockedKernel();

            // subscribe kernel for event
            bool hasBeenLoaded = false;
            kernel.EventAggregator.Subscribe<NomadAllModulesLoadedMessage>((message) =>
            {
                Assert.AreNotEqual(0,message.ModuleInfos.Count());
                hasBeenLoaded = true;
            });

            //  compiling simple modules
            const string dir = @"Modules\Kernel\SimpleAllModulesLoadedAwarenessTestModules\Simple\";
            const string fileName = "AllModulesLoadedEventAwareModule.dll";
            const string module1Name = "SimpleModule1.dll";
            const string module2Name = "SimpleModule2.dll";
            const string keyFile = @"newkey.xml";

            CompileSimpleModules(dir, keyFile, module1Name, module2Name);

            // compiling aware module
            var compiler = new ModuleCompiler
                               {
                                   OutputDirectory =
                                       Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                    @"Modules\Kernel\SimpleAllModulesLoadedAwarenessTestModules\Aware\")
                               };

            compiler.OutputName = Path.Combine(compiler.OutputDirectory, fileName);

            string modulePath = compiler.GenerateModuleFromCode(
                @"..\Source\Nomad.Tests\FunctionalTests\Data\Kernel\AllModulesLoadedEventAwareModule.cs");

            KeysGeneratorProgram.Main(new[] {keyFile});
            compiler.GenerateManifestForModule(modulePath, keyFile);
            // set up module which subscribes for event

            // loading aware module
            IModuleDiscovery setUpDiscovery = SetUpDiscovery(new ModuleInfo(modulePath));
            Assert.DoesNotThrow(() => kernel.LoadModules(setUpDiscovery));

            // loading simple modules
            var directoryDiscovery = new DirectoryModuleDiscovery(dir, SearchOption.TopDirectoryOnly);
            Assert.DoesNotThrow(() => kernel.LoadModules(directoryDiscovery));

            //verify the method being called in a module.
            var carrier = (MessageCarrier) kernel.ModuleAppDomain.CreateInstanceAndUnwrap(
                typeof (MessageCarrier).Assembly.FullName, typeof (MessageCarrier).FullName);

            Assert.AreEqual(
                new[] {"AllModulesLoadedEventAwareModule", "AllModulesLoadedEventAwareModule"},
                carrier.List.ToArray());
            Assert.IsTrue(hasBeenLoaded);
        }


        private static void CompileSimpleModules(string dir, string keyFile,
                                                 string simpleModule1Name, string simpleModule2Name)
        {
            var compiler = new ModuleCompiler
                               {
                                   OutputDirectory =
                                       Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir)
                               };

            KeysGeneratorProgram.Main(new[] {keyFile});

            // generating not subcribed modules
            compiler.OutputName = Path.Combine(compiler.OutputDirectory, simpleModule1Name);
            string module1Path =
                compiler.GenerateModuleFromCode(ModuleCompiler.DefaultSimpleModuleSource);
            compiler.GenerateManifestForModule(module1Path, keyFile);

            compiler.OutputName = Path.Combine(compiler.OutputDirectory, simpleModule2Name);
            string module2Path =
                compiler.GenerateModuleFromCode(ModuleCompiler.DefaultSimpleModuleSourceAlternative);
            compiler.GenerateManifestForModule(module2Path, keyFile);
        }
    }
}