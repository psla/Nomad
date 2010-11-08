using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Nomad.KeysGenerator;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Filters;
using NUnit.Framework;

namespace Nomad.Tests.FunctionalTests.Modules
{
    public class ModuleLoadingWithCompilerTestFixture : MarshalByRefObject
    {
        private const string KeyFile = @"alaMaKota.xml";
        private ModuleManager Manager;
        private ModuleCompiler _moduleCompiler;
        protected AppDomain Domain { get; private set; }


        [TestFixtureSetUp]
        public void set_up_fixture()
        {
            if (File.Exists(KeyFile))
            {
                File.Delete(KeyFile);
            }
            KeysGeneratorProgram.Main(new[] {KeyFile});
        }


        [SetUp]
        public void set_up()
        {
            _moduleCompiler = new ModuleCompiler();

            Domain = AppDomain.CreateDomain("TEST DOMAIN",
                                            new Evidence(AppDomain.CurrentDomain.Evidence),
                                            AppDomain.CurrentDomain.BaseDirectory, ".",
                                            true);

            // create loader in another app domain
            var containerCreator =
                (ContainerCreator)
                Domain.CreateInstanceAndUnwrap(typeof (ContainerCreator).Assembly.FullName,
                                               typeof (ContainerCreator).FullName);
            IModuleLoader moduleLoader = containerCreator.CreateModuleLoaderInstance();

            Manager = new ModuleManager(moduleLoader, new CompositeModuleFilter(),
                                        new DependencyChecker());
        }


        protected void LoadModulesFromDiscovery(IModuleDiscovery discovery)
        {
            Manager.LoadModules(discovery);
        }


        protected void AssertModulesLoadedAreEqualTo(params string[] expectedModuleNames)
        {
            // cross domain communication is really painful
            var carrier =
                (MessageCarrier)
                Domain.CreateInstanceAndUnwrap(typeof (MessageCarrier).Assembly.FullName,
                                               typeof (MessageCarrier).FullName);

            string[] modules = carrier.List.ToArray();
            Assert.That(modules, Is.EqualTo(expectedModuleNames));
        }


        protected void SetUpModuleWithManifest(string outputDirectory, string srcPath,
                                               params string[] references)
        {
            _moduleCompiler.OutputDirectory = outputDirectory;

            string modulePath = _moduleCompiler.GenerateModuleFromCode(srcPath, references);

            // copy the references into folder with 
            foreach (string reference in references)
            {
                File.Copy(reference, Path.Combine(outputDirectory, Path.GetFileName(reference)));
            }

            // manifest generating is for folder
            _moduleCompiler.GenerateManifestForModule(modulePath, KeyFile);

            // remove those references
            foreach (string reference in references)
            {
                File.Delete(Path.Combine(outputDirectory, Path.GetFileName(reference)));
            }
        }

        #region Nested type: MessageCarrier

        private class MessageCarrier : MarshalByRefObject
        {
            private readonly IList<string> _list;


            public MessageCarrier()
            {
                _list = LoadedModulesRegistry.GetRegisteredModules()
                    .Select(type => type.Name).ToList();
            }


            public IEnumerable<string> List
            {
                get { return _list; }
            }
        }

        #endregion
    }
}