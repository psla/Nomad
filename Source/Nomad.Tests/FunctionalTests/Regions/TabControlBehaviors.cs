using System.Collections;
using System.Threading;
using Nomad.Regions;
using Nomad.Regions.Adapters;
using NUnit.Framework;
using TestsShared.FunctionalTests;
using WiPFlash.Framework;

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
            var adapters = new IRegionAdapter[] {new TabControlAdapter()};
            var regionFactory = new RegionFactory(adapters);
            _regionManager = new RegionManager(regionFactory);
        }


        [Test]
        public void can_attach_region()
        {
            IRegion region = null;
            _guiTestRunner.Invoke(
                () =>
                region = _regionManager.AttachRegion("region", _guiTestRunner.Window.TabControl));
            Assert.IsNotNull(region);
        }


        [Test]
        public void can_add_a_tab()
        {
            _guiTestRunner.Invoke(
                () =>
                    {
                        var region = _regionManager.AttachRegion("region",
                                                                 _guiTestRunner.Window.TabControl);
                        var view = new object();
                        region.AddView(view);
                    });

            _guiTestRunner.Wait();

            _guiTestRunner.WindowAutomation.Find<WiPFlash.Components.Tab>(
                new TitleBasedFinder(), "System.Object");
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
            _guiTestRunner.Wait();

            var tab = _guiTestRunner.WindowAutomation.Find<WiPFlash.Components.Tab>(
                new TitleBasedFinder(), "tab1");
            tab.Select();

            Assert.Contains("tab1", (ICollection) region.ActiveViews);
        }

        [Test]
        [Ignore]
        public void synchronizes_active_item_when_program_activates()
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

                    region.Activate("tab1");
                });
            _guiTestRunner.Wait();

            var tab = _guiTestRunner.WindowAutomation.Find<WiPFlash.Components.Tab>(
                new TitleBasedFinder(), "tab1");

            // TODO: test whether it is actually selected
        }
    }
}