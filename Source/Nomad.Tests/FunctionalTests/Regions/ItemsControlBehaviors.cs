using System.Collections;
using System.Linq;
using System.Windows.Controls;
using Moq;
using Nomad.Regions;
using Nomad.Regions.Adapters;
using NUnit.Framework;
using TestsShared;
using TestsShared.FunctionalTests;
using White.Core.UIItems.TabItems;

namespace Nomad.Tests.FunctionalTests.Regions
{
    [FunctionalTests]
    public class ItemsControlBehaviors : GuiTestFixture<FakeWindowWithRegions>
    {
        private ItemsControl _itemsControl;
        private RegionManager _regionManager;


        [TestFixtureSetUp]
        public void show_window()
        {
            Run();
            _itemsControl = Window.ItemsControl;
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
            Invoke(() => region = _regionManager.AttachRegion("region", _itemsControl));
            Assert.IsNotNull(region);
        }


        [Test]
        public void can_add_an_item_and_it_is_active_by_default()
        {
            Invoke(
                () =>
                    {
                        IRegion region = _regionManager.AttachRegion("region", _itemsControl);
                        string view = "tab1";
                        region.AddView(view);
                    });

            Wait();

            Assert.IsNotNull(_itemsControl.Items[0]);
        }


        [Test]
        public void multiple_views_are_visible()
        {
            IRegion region = null;
            Invoke(
                () =>
                    {
                        region = _regionManager.AttachRegion("region",
                                                             _itemsControl);
                        region.AddView("tab1");
                        region.AddView("tab2");
                        region.AddView("tab3");
                    });
           
            Wait();

            Assert.AreEqual(3, _itemsControl.Items.Count, "expected all items to be active!");
        }

        /* TODO: Implement synchronize active views 
        [Test]
        public void deactivates_items_properly()
        {
            Invoke(
                () =>
                    {
                        IRegion region = _regionManager.AttachRegion("region", _itemsControl);
                        var view2 = "tab2";
                        region.AddView("tab1");
                        region.AddView(view2);
                        region.AddView("tab3");
                        region.Deactivate(view2);
                    });
            Wait();

            Assert.IsFalse(_itemsControl.Items.Cast<IEnumerable>().Any(x => x == "tab2"));
        } */
    }
}