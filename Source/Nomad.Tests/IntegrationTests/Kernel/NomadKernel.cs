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
            var moduleFilterMock = new Mock<IModuleFilter>(MockBehavior.Loose);

            var configuration = new NomadConfiguration();
            configuration.ModuleFilter = moduleFilterMock.Object;
            
            //TODO: write ijecting into nomad configuratin and check if it really works... if is freezed
        }

      
    }
}