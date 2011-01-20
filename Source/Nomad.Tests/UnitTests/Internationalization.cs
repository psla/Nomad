using System.Threading;
using Moq;
using Nomad.Internationalization;
using NUnit.Framework;

namespace Nomad.Tests.UnitTests
{
    public class Internationalization
    {
        private ResourceProvider _resourceProvider;


        [SetUp]
        public void setup()
        {
            _resourceProvider = new ResourceProvider();
        }

        [Test]
        public void returns_request_when_no_resource_matching_language()
        {
            var request = "test-resource";
            var result = _resourceProvider.Retrieve(request);
            Assert.AreEqual(request, result);
        }

        [Test]
        public void returns_string_from_resource_if_exists()
        {
            var request = "test-resource";
            var response = "Translated test resource";
            var resourceSource = new Mock<IResourceSource>();
            resourceSource.Setup(x => x.Retrieve(It.Is<string>(y => y == request))).Returns(response);
            _resourceProvider.AddSource(Thread.CurrentThread.CurrentCulture.Name, resourceSource.Object);
            var result = _resourceProvider.Retrieve(request);
            Assert.AreEqual(response, result);
        }

        [Test]
        public void returns_request_when_no_resource_language()
        {
            var request = "test-resource";
            var resourceSource = new Mock<IResourceSource>();
            _resourceProvider.AddSource(Thread.CurrentThread.CurrentCulture.Name, resourceSource.Object);
            var response = _resourceProvider.Retrieve(request);
            Assert.AreEqual(request, response);
        }
    }
}