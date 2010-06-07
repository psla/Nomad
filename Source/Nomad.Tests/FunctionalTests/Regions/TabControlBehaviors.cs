using Nomad.Regions;
using NUnit.Framework;
using TestsShared.FunctionalTests;

namespace Nomad.Tests.FunctionalTests.Regions
{
    public class TabControlBehaviors
    {
        private GuiTestRunner<FakeWindowWithRegions> _guiTestRunner;
        private RegionManager _regionManager;


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


        [SetUp]
        public void set_up()
        {
            var adapters = new IRegionAdapter[] {};
            var regionFactory = new RegionFactory(adapters);
            _regionManager = new RegionManager(regionFactory);
        }


        [Test]
        public void can_attach_region()
        {
            var region = _regionManager.AttachRegion("region", _guiTestRunner.Window.TabControl);
            Assert.IsNotNull(region);
        }

        
        [Test]
        public void can_add_a_tab()
        {
            
        }
    }
}