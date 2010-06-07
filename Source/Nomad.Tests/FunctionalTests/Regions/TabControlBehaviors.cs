using System.Threading;
using NUnit.Framework;
using TestsShared.FunctionalTests;

namespace Nomad.Tests.FunctionalTests.Regions
{
    public class TabControlBehaviors
    {
        private GuiTestRunner<FakeWindowWithRegions> _guiTestRunner;
            

        [TestFixtureSetUp]
        public void show_window()
        {
            _guiTestRunner = new GuiTestRunner<FakeWindowWithRegions>();
            _guiTestRunner.Run();
        }


        [TestFixtureTearDown]
        public void close_window()
        {
            _guiTestRunner.Stop();
        }

        [Test]
        public void can_do_nothing()
        {
            Thread.Sleep(10000);
        }
    }
}