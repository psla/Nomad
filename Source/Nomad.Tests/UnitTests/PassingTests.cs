using NUnit.Framework;

namespace Nomad.Tests.UnitTests
{
    [TestFixture]
    [Category("UnitTests")]
    public class PassingTests
    {
        [Test]
        public void pass()
        {
            Assert.Pass();
        }
    }
}