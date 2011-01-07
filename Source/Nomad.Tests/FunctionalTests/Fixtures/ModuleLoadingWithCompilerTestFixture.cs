using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nomad.Core;
using Nomad.KeysGenerator;
using Nomad.Modules;
using Nomad.Modules.Discovery;
using Nomad.Modules.Filters;
using Nomad.Tests.FunctionalTests.Modules;
using Nomad.Utils.ManifestCreator;
using Nomad.Utils.ManifestCreator.DependenciesProvider;
using NUnit.Framework;

namespace Nomad.Tests.FunctionalTests.Fixtures
{
    public class ModuleLoadingWithCompilerTestFixture : MarshalByRefObject
    {
        private const string KeyFile = @"alaMaKota.xml";
        private ModuleCompiler _moduleCompiler;

        protected NomadKernel Kernel;
        protected AppDomain Domain;


        [TestFixtureSetUp]
        public virtual void SetUpFixture()
        {
            if (File.Exists(KeyFile))
            {
                File.Delete(KeyFile);
            }
            KeysGeneratorProgram.Main(new[] {KeyFile});
        }

        [TestFixtureTearDown]
        public virtual void CleanUpFixture()
        {
            if (File.Exists(KeyFile))
            {
                File.Delete(KeyFile);
            }
        }


        [SetUp]
        public virtual void SetUp()
        {
            _moduleCompiler = new ModuleCompiler();

            // prepare configuration
            NomadConfiguration configuration = NomadConfiguration.Default;
            configuration.ModuleFilter = new CompositeModuleFilter();
            configuration.DependencyChecker = new DependencyChecker();

            // initialize kernel
            Kernel = new NomadKernel(configuration);

            // domain
            Domain = Kernel.ModuleAppDomain;
        }


        protected void LoadModulesFromDiscovery(IModuleDiscovery discovery)
        {
            Kernel.LoadModules(discovery);
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
            // NOTE: we are using whole directory module discovery instead of file one
            var configuration = ManifestBuilderConfiguration.Default;
            configuration.ModulesDependenciesProvider =new WholeDirectoryModulesDependenciesProvider();

            SetUpModuleWithManifest(outputDirectory,srcPath,configuration,references);
        }

        protected void SetUpModuleWithManifest(string outputDirectory, string srcPath, ManifestBuilderConfiguration configuration,
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
            _moduleCompiler.GenerateManifestForModule(modulePath, KeyFile,configuration);

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