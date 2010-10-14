using Nomad.Core;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Kernel
{
    [IntegrationTests]
    public class NomadKernelTests
    {
        private NomadKernel _nomadKernel;
     
        [Test]
        public void injecting_nomad_configuration_freezes_the_configuration()
        {
            //Use default
            var configuration = new NomadConfiguration();
            
            Assert.IsFalse(configuration.IsFrozen);
            _nomadKernel = new NomadKernel(configuration);
            Assert.IsTrue(configuration.IsFrozen);
        }

        [Test]
        public void injecting_default_nomad_configuration_works_well()
        {
            //Use default
            var configuration = new NomadConfiguration();
            Assert.DoesNotThrow(() => _nomadKernel = new NomadKernel(configuration));
        }
      
    }
}