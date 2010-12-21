using System.Collections;
using System.Windows.Controls;
using Moq;
using Nomad.Communication.EventAggregation;
using Nomad.Regions;
using Nomad.Regions.Adapters;
using Nomad.Regions.Behaviors;
using NUnit.Framework;
using TestsShared;
using TestsShared.FunctionalTests;
using White.Core.UIItems.TabItems;

namespace Nomad.Tests.FunctionalTests.Regions
{
    [FunctionalTests]
    public class RegisteringRegionsFromMarkup : GuiTestFixture<FakeWindowWithRegionsRegisteredByAttachedProperty>
    {
        private RegionManager _regionManager;
        private TabControl _tabControl;


        [TestFixtureSetUp]
        public void show_window()
        {
            set_up();

            Run();
            _tabControl = Window.TabControl;
        }


        [TestFixtureTearDown]
        public void close_window()
        {
            Stop();
        }


        private void set_up()
        {
            var adapters = new IRegionAdapter[] {new TabControlAdapter()};
            var regionFactory = new RegionFactory(adapters);
            _regionManager = new RegionManager(regionFactory);
        }


        [Test]
        public void is_automatically_attached()
        {
            IRegion region = null;
            region = _regionManager.GetRegion("tabs");
            Assert.IsNotNull(region, "Expected region to be registered in regionmanager");
        }
    }
}