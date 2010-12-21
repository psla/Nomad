using System.Windows.Controls.Primitives;
using Nomad.Regions;
using Nomad.Regions.Adapters;
using NUnit.Framework;
using TestsShared;
using TestsShared.FunctionalTests;

namespace Nomad.Tests.FunctionalTests.Regions
{
    [FunctionalTests]
    public class StatusBarBehaviors : GuiTestFixture<FakeWindowWithRegions>
    {
        private RegionManager _regionManager;
        private StatusBar _tabControl;


        [TestFixtureSetUp]
        public void show_window()
        {
            Run();
            _tabControl = Window.StatusBar;
        }


        [TestFixtureTearDown]
        public void close_window()
        {
            Stop();
        }


        [SetUp]
        public void set_up()
        {
            var adapters = new IRegionAdapter[] { new StatusBarAdapter() };
            var regionFactory = new RegionFactory(adapters);
            _regionManager = new RegionManager(regionFactory);
        }


        [Test]
        public void can_attach_region()
        {
            IRegion region = null;
            Invoke(() => region = _regionManager.AttachRegion("region", _tabControl));
            Assert.IsNotNull(region);
        }

    }
}