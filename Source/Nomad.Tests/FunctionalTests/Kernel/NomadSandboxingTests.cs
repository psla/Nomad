using System.IO;
using Nomad.Core;
using Nomad.Modules;
using Nomad.Modules.Filters;
using Nomad.Tests.FunctionalTests.Fixtures;
using TestsShared;
using NUnit.Framework;

namespace Nomad.Tests.FunctionalTests.Kernel
{
    [FunctionalTests]
    public class NomadSandboxingTests : Fixtures.ModuleLoadingWithCompilerTestFixture
    {
        [TestFixtureTearDown]
        public override void CleanUpFixture()
        {
            base.CleanUpFixture();
            Kernel.UnloadModules();
            Directory.Delete(@"Modules\Services\", true);
        }

        [TestFixtureSetUp]
        public override void SetUpFixture()
        {
            base.SetUpFixture();

        }

        [SetUp]
        public override void SetUp()
        {
            // prepare configuration
            NomadConfiguration configuration = NomadConfiguration.Default;
            configuration.ModuleFilter = new CompositeModuleFilter();
            configuration.DependencyChecker = new DependencyChecker();

            // initialize kernel
            Kernel = new NomadKernel(configuration);

            // domain
            Domain = Kernel.ModuleAppDomain;
        }
    }
}