using NUnit.Framework;

namespace TestsShared.Tests
{
    [TestFixture]
    public class FakeTests
    {
        ///<summary>
        /// Automatic pass test
        ///</summary>
        [Test]
        public void Passtest ()
        {
            Assert.Pass ("Passed test passed");
        }

        ///<summary>
        /// If build runs after push
        ///</summary>
        [Test]
        public void PostReceiveTest()
        {
            Assert.Pass("Checking if build runs after push");
        }
    }
}