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
using WiPFlash.Framework;

namespace Nomad.Tests.FunctionalTests.Regions
{
    [FunctionalTests]
    public class TabControlBehaviors
    {
        private GuiTestRunner<FakeWindowWithRegions> _guiTestRunner;
        private RegionManager _regionManager;
        private TabControl _tabControl;


        [TestFixtureSetUp]
        public void show_window()
        {
            _guiTestRunner = new GuiTestRunner<FakeWindowWithRegions>();
            _guiTestRunner.Run();
            _tabControl = _guiTestRunner.Window.TabControl;
        }


        [TestFixtureTearDown]
        public void close_window()
        {
            _guiTestRunner.Stop();
        }


        [SetUp]
        public void set_up()
        {
            var adapters = new IRegionAdapter[] {new TabControlAdapter()};
            var regionFactory = new RegionFactory(adapters);
            _regionManager = new RegionManager(regionFactory);
        }


        [Test]
        public void can_attach_region()
        {
            IRegion region = null;
            _guiTestRunner.Invoke(() => region = _regionManager.AttachRegion("region", _tabControl));
            Assert.IsNotNull(region);
        }


        [Test]
        public void can_add_a_tab()
        {
            _guiTestRunner.Invoke(
                () =>
                    {
                        var region = _regionManager.AttachRegion("region", _tabControl);
                        var view = "tab1";
                        region.AddView(view);
                    });

            _guiTestRunner.Wait();

            _guiTestRunner.WindowAutomation.Find<WiPFlash.Components.Tab>(
                new TitleBasedFinder(), "tab1");
        }


        [Test]
        public void synchronizes_active_item_when_user_clicks()
        {
            IRegion region = null;
            _guiTestRunner.Invoke(
                () =>
                    {
                        region = _regionManager.AttachRegion("region",
                                                             _guiTestRunner.Window.TabControl);
                        region.AddView("tab1");
                        region.AddView("tab2");
                        region.AddView("tab3");
                    });
            
            var tab = _guiTestRunner.WindowAutomation.Find<WiPFlash.Components.Tab>(
                new TitleBasedFinder(), "tab1");
            tab.Select();            
            _guiTestRunner.Wait();

            Assert.Contains("tab1", (ICollection) region.ActiveViews);
        }


        [Test]
        public void synchronizes_active_item_when_program_activates()
        {
            _guiTestRunner.Invoke(
                () =>
                    {
                        var region = _regionManager.AttachRegion("region", _tabControl);
                        region.AddView("tab1");
                        region.AddView("tab2");
                        region.AddView("tab3");

                        region.Activate("tab2");
                    });
            _guiTestRunner.Wait();

            object selectedItem = null;
            _guiTestRunner.Invoke(() => selectedItem = _tabControl.SelectedItem);
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

            _guiTestRunner.Invoke(
                () =>
                {
                    var region = _regionManager.AttachRegion("region", _tabControl);
                    region.AddView(viewMock.Object);
                    region.AddView("tab2");
                    region.AddView("tab3");

                    region.Activate("tab2");
                });
            _guiTestRunner.Wait();

            Assert.IsFalse(lastIsActive);
            
            var tab = _guiTestRunner.WindowAutomation.Find<WiPFlash.Components.Tab>(
                new TitleBasedFinder(), viewMock.Object.GetType().FullName);
            tab.Select();
            _guiTestRunner.Wait();

            Assert.IsTrue(lastIsActive);
        }
    }
}