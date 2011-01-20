using System;
using System.Globalization;
using System.Threading;
using Moq;
using Nomad.Internationalization;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests
{
    [UnitTests]
    public class Internationalization
    {
        private ResourceProvider _resourceProvider;


        [SetUp]
        public void setup()
        {
            _resourceProvider = ResourceProvider.CurrentResourceProvider;
            _resourceProvider.Reset();
        }


        [Test]
        public void returns_request_when_no_resource_matching_language()
        {
            string request = "test-resource";
            string result = _resourceProvider.Retrieve(request);
            Assert.AreEqual(request, result);
        }


        [Test]
        public void returns_string_from_resource_if_exists()
        {
            string request = "test-resource";
            string response = "Translated test resource";
            var resourceSource = new Mock<IResourceSource>();
            resourceSource.Setup(x => x.Retrieve(It.Is<string>(y => y == request))).Returns(response);
            _resourceProvider.AddSource(Thread.CurrentThread.CurrentCulture.Name,
                                        resourceSource.Object);
            string result = _resourceProvider.Retrieve(request);
            Assert.AreEqual(response, result);
        }


        [Test]
        public void returns_request_when_no_resource_language()
        {
            string request = "test-resource";
            var resourceSource = new Mock<IResourceSource>();
            _resourceProvider.AddSource(Thread.CurrentThread.CurrentCulture.Name,
                                        resourceSource.Object);
            string response = _resourceProvider.Retrieve(request);
            Assert.AreEqual(request, response);
        }


        [Test]
        public void changes_ui_culture_correctly()
        {
            var newUiCulture = new CultureInfo("en-GB");
            _resourceProvider.ChangeUiCulture(newUiCulture);

            Assert.AreEqual(Thread.CurrentThread.CurrentUICulture, newUiCulture);
        }


        [Test]
        public void ui_culture_changes_fires_event()
        {
            EventArgs cultureChanged = null;
            var newUiCulture = new CultureInfo("en-GB");
            _resourceProvider.CultureChanged += (x, y) => cultureChanged = y;
            _resourceProvider.ChangeUiCulture(newUiCulture);

            Assert.IsNotNull(cultureChanged, "Event should be fired after changing language");
        }

        [Test]
        public void may_be_defined_many_sources_for_one_language_and_all_are_used_in_any_order()
        {
            string request = "test-resource";
            string response = "Translated test resource";
            
            var resourceSource = new Mock<IResourceSource>();
            resourceSource.Setup(x => x.Retrieve(It.Is<string>(y => y == request))).Returns(response);

            string request2 = "test-resource2";
            string response2 = "Another test resource";
            var resourceSource2 = new Mock<IResourceSource>();
            resourceSource2.Setup(x => x.Retrieve(It.Is<string>(y => y == request2))).Returns(response2);

            _resourceProvider.AddSource(Thread.CurrentThread.CurrentCulture.Name,resourceSource.Object);
            _resourceProvider.AddSource(Thread.CurrentThread.CurrentCulture.Name,resourceSource2.Object);

            var result1 = _resourceProvider.Retrieve(request);
            var result2 = _resourceProvider.Retrieve(request2);
            Assert.AreEqual(response, result1);
            Assert.AreEqual(response2, result2);
        }
    }
}