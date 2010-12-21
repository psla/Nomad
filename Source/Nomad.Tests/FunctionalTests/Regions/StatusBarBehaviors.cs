using System.Windows.Controls.Primitives;
using Nomad.Regions;
using Nomad.Regions.Adapters;
using NUnit.Framework;
using TestsShared;
using TestsShared.FunctionalTests;
using White.Core.UIItems;
using White.Core.UIItems.TabItems;
using White.Core.UIItems.WindowStripControls;

namespace Nomad.Tests.FunctionalTests.Regions
{
    [FunctionalTests]
    public class StatusBarBehaviors : GuiTestFixture<FakeWindowWithRegions>
    {
        private RegionManager _regionManager;
        private StatusBar _statusBar;


        [TestFixtureSetUp]
        public void show_window()
        {
            Run();
            _statusBar = Window.StatusBar;
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
            Invoke(() => region = _regionManager.AttachRegion("region", _statusBar));
            Assert.IsNotNull(region);
        }

        [Test]
        public void can_add_a_status_element()
        {
            Invoke(
                () =>
                {
                    var region = _regionManager.AttachRegion("region", _statusBar);
                    var view = "statusBarElement1";
                    region.AddView(view);
                });

            Wait();

            var statusBar = WhiteWindow.Get<WPFStatusBar>("StatusBar");
            Wait();
            Assert.AreEqual(1, statusBar.Items.Count);
        }

    }
}