using System.Security.Authentication;
using Moq;
using Nomad.AssemblyLoading;
using Nomad.Signing;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.AssemblyLoading
{
    [TestFixture]
    [UnitTests]
    public class SignedAssemblyLoaderTests
    {
        private IAssemblyLoader _assemblyLoader;
        private IFileSignatureVerificator _fileSignatureVerificator;


        [SetUp]
        public void SetUp()
        {
            _fileSignatureVerificator = new FileSignatureVerificator();
            _assemblyLoader = new SignedAssemblyLoader(new Mock<IAssemblyLoader>().Object,
                                                       _fileSignatureVerificator);
        }


        [Test]
        public void throws_exception_when_none_file_exists()
        {
            Assert.Throws<InvalidCredentialException>(() => _assemblyLoader.LoadAssembly("test"));
        }
    }
}