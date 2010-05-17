using NUnit.Framework;

namespace TestsShared.Tests
{
    [TestFixture]
    public class FakeTests
    {
        [Test]
        public void FailedTest ()
        {
            Assert.Fail ("Failed test failed");
        }


        [Test]
        public void Passtest ()
        {
            Assert.Pass ("Passed test passed");
        }
    }
}