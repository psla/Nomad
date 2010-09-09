using NUnit.Framework;
using TestsShared.FunctionalTests;

namespace FunctionalTests_NewRunnerWithWhite
{
    public class Fixture2 : GuiTestFixture<Window2>
    {
        [Test]
        public void can_execute_the_test()
        {
            Assert.Pass();
        }
    }
}