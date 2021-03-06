using System.Windows.Controls;
using Nomad.Regions;
using Nomad.Regions.Adapters;
using NUnit.Framework;
using TestsShared;
using TestsShared.FunctionalTests;

namespace Nomad.Tests.FunctionalTests.Regions
{
    [FunctionalTests]
    public class ToolBarBehaviors : GuiTestFixture<FakeWindowWithRegions>
    {
        private RegionManager _regionManager;
        private ToolBar _toolBar;


        [TestFixtureSetUp]
        public void show_window()
        {
            Run();
            _toolBar = Window.Toolbar;
        }


        [TestFixtureTearDown]
        public void close_window()
        {
            Stop();
        }


        [SetUp]
        public void set_up()
        {
            var adapters = new IRegionAdapter[] {new ItemsControlAdapter()};
            var regionFactory = new RegionFactory(adapters);
            _regionManager = new RegionManager(regionFactory);
        }


        [Test]
        public void can_attach_region()
        {
            IRegion region = null;
            Invoke(() => region = _regionManager.AttachRegion("region", _toolBar));
            Assert.IsNotNull(region);
        }


        [Test]
        public void can_add_a_toolbar_element()
        {
            Button view = null;
            Invoke(
                () =>
                    {
                        IRegion region = _regionManager.AttachRegion("region", _toolBar);
                        view = new Button() {Content = "test"};
                        region.AddView(view);
                    });

            Assert.AreEqual(1, Window.Toolbar.Items.Count);
            Assert.AreSame(view, Window.Toolbar.Items[0]);
        }
    }
}