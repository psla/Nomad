using System.Collections;
using Nomad.Regions;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Regions
{
    [UnitTests]
    public class SingleActiveViewRegionBehavior
    {
        private SingleActiveViewRegion _region;


        [SetUp]
        public void set_up()
        {
            _region = new SingleActiveViewRegion();
        }


        [Test]
        public void new_region_has_no_views()
        {
            Assert.IsEmpty((ICollection) _region.Views);
        }


        [Test]
        public void can_add_view()
        {
            var view = new object();
            _region.AddView(view);

            Assert.Contains(view, (ICollection) _region.Views);
        }

        [Test]
        public void can_clear_views()
        {
            var view = new object();
            _region.AddView(view);

            _region.ClearViews();
            
            Assert.IsEmpty((ICollection) _region.Views);
            Assert.IsEmpty((ICollection) _region.ActiveViews);
        }
    }
}