using NUnit.Framework;
using TestsShared.FunctionalTests;

namespace FunctionalTests_NewRunnerWithWhite
{
    public class Fixture1 : GuiTestFixture<Window1>
    {
        [Test]
        public void can_execute_the_test()
        {
            Assert.Pass();
        }
    }
}
