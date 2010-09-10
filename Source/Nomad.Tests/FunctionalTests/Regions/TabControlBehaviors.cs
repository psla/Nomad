using System.Collections;
using System.Threading;
using System.Windows.Controls;
using Moq;
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
    public class TabControlBehaviors
    {
        private GuiTestFixture<FakeWindowWithRegions> _guiTestFixture;
        private RegionManager _regionManager;
        private TabControl _tabControl;


        [TestFixtureSetUp]
        public void show_window()
        {
            _guiTestFixture = new GuiTestFixture<FakeWindowWithRegions>();
            _guiTestFixture.Run();
            _tabControl = _guiTestFixture.Window.TabControl;
        }


        [TestFixtureTearDown]
        public void close_window()
        {
            _guiTestFixture.Stop();
        }


        [SetUp]
        public void set_up()
        {
            var adapters = new IRegionAdapter[] { new TabControlAdapter() };
            var regionFactory = new RegionFactory(adapters);
            _regionManager = new RegionManager(regionFactory);
        }


        [Test]
        public void can_attach_region()
        {
            IRegion region = null;
            _guiTestFixture.Invoke(() => region = _regionManager.AttachRegion("region", _tabControl));
            Assert.IsNotNull(region);
        }


        [Test]
        public void can_add_a_tab()
        {
            _guiTestFixture.Invoke(
                () =>
                {
                    var region = _regionManager.AttachRegion("region", _tabControl);
                    var view = "tab1";
                    region.AddView(view);
                });

            _guiTestFixture.Wait();

            var whiteTab = _guiTestFixture.WhiteWindow.Get<Tab>("TabControl");
            Assert.IsNotNull(whiteTab.Pages.Find(obj => obj.Name.Contains("tab1")));
        }


        [Test]
        public void synchronizes_active_item_when_user_clicks()
        {
            IRegion region = null;
            _guiTestFixture.Invoke(
                () =>
                {
                    region = _regionManager.AttachRegion("region",
                                                         _guiTestFixture.Window.TabControl);
                    region.AddView("tab1");
                    region.AddView("tab2");
                    region.AddView("tab3");
                });

            var whiteTabControl = _guiTestFixture.WhiteWindow.Get<Tab>("TabControl");
            whiteTabControl.SelectTabPage("tab1");
            _guiTestFixture.Wait();

            Assert.Contains("tab1", (ICollection)region.ActiveViews);
        }


        [Test]
        public void synchronizes_active_item_when_program_activates()
        {
            _guiTestFixture.Invoke(
                () =>
                {
                    var region = _regionManager.AttachRegion("region", _tabControl);
                    region.AddView("tab1");
                    region.AddView("tab2");
                    region.AddView("tab3");

                    region.Activate("tab2");
                });
            _guiTestFixture.Wait();

            object selectedItem = null;
            _guiTestFixture.Invoke(() => selectedItem = _tabControl.SelectedItem);
            Assert.AreEqual("tab2", selectedItem);
        }

        [Test]
        public void active_aware_view_is_notified_when_it_is_activated()
        {
            var viewMock = new Mock<IActiveAware>();
            bool lastIsActive = false;

            viewMock.Setup(aware => aware.SetIsActive(It.IsAny<bool>()))
                .Callback<bool>(isActive => lastIsActive = isActive)
                .Verifiable();

            viewMock.Setup(aware => aware.ToString())
                .Returns("ActiveAware");

            _guiTestFixture.Invoke(
                () =>
                {
                    var region = _regionManager.AttachRegion("region", _tabControl);
                    region.AddView(viewMock.Object);
                    region.AddView("tab2");
                    region.AddView("tab3");

                    region.Activate("tab2");
                });
            _guiTestFixture.Wait();

            Assert.IsFalse(lastIsActive);
            var whiteTab = _guiTestFixture.WhiteWindow.Get<Tab>("TabControl");
            var tab = whiteTab.Pages.Find(obj => obj.Name.Contains(viewMock.Object.GetType().FullName));
            tab.Select();
 
            Assert.IsTrue(lastIsActive);
        }
    }
}