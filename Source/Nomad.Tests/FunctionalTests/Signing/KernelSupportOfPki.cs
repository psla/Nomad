using Nomad.Core;
using Nomad.Signing.SignatureProviders;
using NUnit.Framework;

namespace Nomad.Tests.FunctionalTests.Signing
{
    public class KernelSupportOfPki
    {
        [Test]
        public void by_default_kernel_configuration_should_have_no_restriction_on_signatures()
        {
            var kernelConfiguration = new NomadConfiguration();
            Assert.IsInstanceOf(typeof(SignatureProvider), kernelConfiguration.SignatureProvider);
            
        }
    }
}