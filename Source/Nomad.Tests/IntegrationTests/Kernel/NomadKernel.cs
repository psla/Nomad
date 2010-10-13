using Moq;
using Nomad.Core;
using Nomad.Modules;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.IntegrationTests.Kernel
{
    [IntegrationTests]
    public class NomadKernel
    {
        private NomadKernel _nomadKernel;
     
        [Test]
        public void injecting_nomad_configuration()
        {
            var configurationMock = new Mock<NomadConfiguration>(MockBehavior.Loose);

            //TODO: write injecting into nomad configuratin and check if it really works... if is freezed
        }

      
    }
}