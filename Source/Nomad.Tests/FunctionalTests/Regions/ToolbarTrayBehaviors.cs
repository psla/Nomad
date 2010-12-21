using System.Windows.Controls;
using Nomad.Regions;
using NUnit.Framework;
using TestsShared;
using TestsShared.FunctionalTests;

namespace Nomad.Tests.FunctionalTests.Regions
{
    [FunctionalTests]
    public class ToolBarTrayBehaviors : GuiTestFixture<FakeWindowWithRegions>
    {
        private RegionManager _regionManager;
        private ToolBarTray _toolBarTray;


        [TestFixtureSetUp]
        public void show_window()
        {
            Run();
            _toolBarTray = Window.ToolBarTray;
        }


        [TestFixtureTearDown]
        public void close_window()
        {
            Stop();
        }


        [SetUp]
        public void set_up()
        {
            var adapters = new IRegionAdapter[] {new ToolbarTrayAdapter()};
            var regionFactory = new RegionFactory(adapters);
            _regionManager = new RegionManager(regionFactory);
        }


        [Test]
        public void can_attach_region()
        {
            IRegion region = null;
            Invoke(() => region = _regionManager.AttachRegion("region", _toolBarTray));
            Assert.IsNotNull(region);
        }


        [Test]
        public void can_add_a_toolbar_element()
        {
            ToolBar view = null;
            Invoke(
                () =>
                    {
                        IRegion region = _regionManager.AttachRegion("region", _toolBarTray);
                        view = new ToolBar();
                        region.AddView(view);
                    });

            Assert.AreEqual(1, Window.ToolBarTray.ToolBars.Count);
            Assert.AreSame(view, Window.ToolBarTray.ToolBars[0]);
        }
    }
}